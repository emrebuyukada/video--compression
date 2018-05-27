using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AForge.Video.FFMPEG;
using WMPLib;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace AForgeGoruntuIsleme
{
    public partial class AnaForm : Form
    { 
        OpenFileDialog ofdVideoYukle = new OpenFileDialog();            
        FolderBrowserDialog fbdFramePath = new FolderBrowserDialog();
        int _TotalFrameCount = 0;
        int _VideoFPS = 0;
        int videoGenislik;
        int videoYukseklik;
        private long inputFileLength = 0;
        private long outBytes = 0;
        private double stretchX = 1.0;
        private double stretchY = 1.0;
        private string uzanti = ".jpg";
        private string uzanti2 = ".mp4";
        Thread frameParcala;
        int FrameSayisi = 0;
        int kosulAynimi = 0;
        List<Bitmap> videoFrameList = new List<Bitmap>();
        public AnaForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

        }//constructor
        public void WriteMPEGSequence(Bitmap img,string yol)
        {
          
            int i;
            int j, k1, k2;
            long j2;
            byte tempByte;
            outBytes = 0;
            int ACSIZE = 1764;
            byte[] leftoverBits = new byte[10];
            byte[] DCbits = new byte[24];
            byte[] ACbits = new byte[ACSIZE];

            int DCY, DCCR, DCCB, lastDCY, lastDCCR, lastDCCB;
            int hblock, vblock;
            byte[,] Y = new byte[16, 16];
            byte[,] CR = new byte[8, 8];
            byte[,] CB = new byte[8, 8];
            byte[,] block = new byte[8, 8];
            double[,] S = new double[8, 8];
            int[,] Q = new int[8, 8];
            int[] ZZ = new int[64];
            long imageBytes = 0;
    
            long bitRate;

            MPEGFunctions MPEG = new MPEGFunctions();

            // Frame Boyutlarını Alıyoruz
            
            string fileName;
            imageBytes = img.Height * img.Width * 3;




          //  Kodlanmış görüntüyü yazmak için Çıktı dosyası ve Bellek dosyası oluştur
            BinaryWriter bw = new BinaryWriter(File.Create(yol));
            MemoryStream ms = new MemoryStream();
            FileInfo info = new FileInfo(yol);

            this.Cursor = Cursors.WaitCursor;

            //	Framein  MPEG çerçevesini kodlamak için değişkenleri ayarlayın
            lastDCY = 128;
            lastDCCR = lastDCCB = 128;
            for (i = 0; i < 10; i++)
                leftoverBits[i] = 255;
            for (i = 0; i < 24; i++)
                ACbits[i] = 255;
            for (i = 0; i < 24; i++)
                DCbits[i] = 255;

            outBytes = 20;

            //MPEG Frame Başlığını MemoryStream'e yaz
            for (i = 0; i < 10; i++)
                MPEG.picHeaderBits[i + 32] = (byte)((0 & (int)Math.Pow(2, 9 - i)) >> (9 - i));
            MPEG.writeToMS(leftoverBits, MPEG.picHeaderBits, ACbits, ref outBytes);
            MPEG.writeToMS(leftoverBits, MPEG.sliceHeaderBits, ACbits, ref outBytes);
            //Framedeki her bir   6x16 piksel bloğu için bunu yapın
            
            for (vblock = 0; vblock < img.Height / 16; vblock++)
                for (hblock = 0; hblock < img.Width / 16; hblock++)
                {

                    // Macroblock üstbilgisi için 2 bitleri artıkları ekleyin.
                     //	artıkbitler = '1', '1';
                     MPEG.writeMbHeader(leftoverBits);

                    //Y [] dizisini 16x16 bloğu RGB değerleri ile doldurun

                    Y = MPEG.getYMatrix(img, vblock, hblock);
                    // Alt örneklemeyle CR ve CB dizilerini 8x8 blok ile doldurun
                    //	RGB dizisi
                    CR = MPEG.getCRMatrix(img, vblock, hblock);
                    CB = MPEG.getCBMatrix(img, vblock, hblock);

                    //Önce 4 Y bloğu için DCT'leri hesaplayın
                    for (k1 = 0; k1 < 2; k1++)
                        for (k2 = 0; k2 < 2; k2++)
                        {
                            //	Blok [] dizisine 8x8 Y blokları koyun ve
                            //	sonra DCT'yi hesaplayın ve sonucu nicelleştirin
                            for (i = 0; i < 8; i++)
                                for (j = 0; j < 8; j++)
                                    block[i, j] = Y[(k1 * 8 + i), (k2 * 8 + j)];
                            S = MPEG.calculateDCT(block);
                            Q = MPEG.Quantize(S);

                            //Farklı Huffman bölümü DC değer kodları
                            //	DC, DC katsayısının diferansiyel değeridir

                            //	Sonra DC değerini DCHuffmanEncode'a gönderin.
                            for (i = 0; i < 24; i++)
                                DCbits[i] = 255;
                            DCY = Q[0, 0] - lastDCY;
                            lastDCY += DCY;
                            DCbits = MPEG.DCHuffmanEncode(DCY, MPEG.DCLumCode, MPEG.DCLumSize);

                          //  AC Huffman değerlerini kodlamak için 
                        //AC katsayılarını ACarray[] içine koy
                        //zikzak sırasına göre, Huffman
                        //sonuç dizisi.
                            for (i = 0; i < ACSIZE; i++)
                                ACbits[i] = 255;
                            ZZ = MPEG.Zigzag(Q);
                            ACbits = MPEG.ACHuffmanEncode(ZZ);

                            //Kodlanmış bitleri MemoryStream'e yaz
                            MPEG.writeToMS(leftoverBits, DCbits, ACbits, ref outBytes);
                        }

                    //Şimdi CB dizisi için DCT'yi hesaplayın ve nicelleştirin
                    S = MPEG.calculateDCT(CB);
                    Q = MPEG.Quantize(S);

                   // DC değerini kodlayın
                    for (i = 0; i < 24; i++)
                        DCbits[i] = 255;
                    DCCB = Q[0, 0] - lastDCCB;
                    lastDCCB += DCCB;
                    DCbits = MPEG.DCHuffmanEncode(DCCB, MPEG.DCChromCode, MPEG.DCChromSize);

                    // AC değerlerini kodlayın
                    for (i = 0; i < ACSIZE; i++)
                        ACbits[i] = 255;
                    ZZ = MPEG.Zigzag(Q);
                    ACbits = MPEG.ACHuffmanEncode(ZZ);

                    // Kodlanmış bitleri MemoryStream'e yaz

                    MPEG.writeToMS(leftoverBits, DCbits, ACbits, ref outBytes);

                    // Şimdi CR dizisi için DCT'yi hesaplayın ve nicelleştirin
                    S = MPEG.calculateDCT(CR);
                    Q = MPEG.Quantize(S);

                    // DC değerini kodlayın
                    for (i = 0; i < 24; i++)
                        DCbits[i] = 255;
                    DCCR = Q[0, 0] - lastDCCR;
                    lastDCCR += DCCR;
                    DCbits = MPEG.DCHuffmanEncode(DCCR, MPEG.DCChromCode, MPEG.DCChromSize);

                    // AC değerlerini kodlayın
                    for (i = 0; i < ACSIZE; i++)
                        ACbits[i] = 255;
                    ZZ = MPEG.Zigzag(Q);
                    ACbits = MPEG.ACHuffmanEncode(ZZ);

                    // Kodlanmış bitleri MemoryStream'e yaz

                    MPEG.writeToMS(leftoverBits, DCbits, ACbits, ref outBytes);
                }

            // MemoryStream'e EOP bitleri yaz
            MPEG.writeEOP(leftoverBits, MPEG.EOPBits);
            outBytes++;

            // Bellek akışını (kodlanmış görüntüyü içeren) arabelleğe koy

            ms = MPEG.getMS();
            byte[] buffer = new Byte[ms.Length];
            buffer = ms.ToArray();

          // Görüntü boyutunu düzeltmek için MPEG  bitlerini ayarlayın
            j = 2048;
            for (i = 0; i < 12; i++)
            {
                MPEG.seqHeaderBits[i + 32] = (byte)((j & img.Width) >> (11 - i));
                MPEG.seqHeaderBits[i + 44] = (byte)((j & img.Height) >> (11 - i));
                j >>= 1;
            }

            // MPEG bitRate değerine ayarlayın

            bitRate = ms.Length * 30 * 8 / 400;
            j2 = 131072;
            for (i = 0; i < 18; i++)
            {
                MPEG.seqHeaderBits[i + 64] = (byte)((j2 & bitRate) >> (17 - i));
                j2 >>= 1;
            }

            //MPEG Sıra üstbilgisini dosyaya yaz
            for (i = 0; i < 12; i++)
            {
                tempByte = 0;
                for (j = 0; j < 8; j++)
                    tempByte = (byte)(tempByte * 2 + MPEG.seqHeaderBits[i * 8 + j]);
                bw.Write(tempByte);
            }

           // MPEG GOP üstbilgisini dosyaya yaz
            for (i = 0; i < 8; i++)
            {
                tempByte = 0;
                for (j = 0; j < 8; j++)
                    tempByte = (byte)(tempByte * 2 + MPEG.GOPHeaderBits[i * 8 + j]);
                bw.Write(tempByte);
            }

            //Her bir MPEG çerçevesi için resim başlığını düzeltin ve yazın
            for (i = 0; i < 1; i++)
            {
                for (j = 0; j < 10; j++)
                    MPEG.picHeaderBits[j + 32] = (byte)((i & (int)Math.Pow(2, 9 - j)) >> (9 - j));
                for (j = 0; j < 4; j++)
                {
                    tempByte = 0;
                    for (k1 = 0; k1 < 8; k1++)
                        tempByte = (byte)(2 * tempByte + MPEG.picHeaderBits[j * 8 + k1]);
                    buffer[j] = tempByte;
                }
                bw.Write(buffer);
            }

            // Üst Bilgi Başlığını sonuna yaz
            bw.Write((byte)0x00);
            bw.Write((byte)0x00);
            bw.Write((byte)0x01);
            bw.Write((byte)0xb7);
            bw.Close();

            this.Cursor = Cursors.Arrow;

            

        }
        private void btnVideoYukle_Click(object sender, EventArgs e)
        {

            DialogResult result =  ofdVideoYukle.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Video Seçmediniz");
                return;
            }//if
            axWindowsMediaPlayer1.URL = ofdVideoYukle.FileName;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }//void
        public void parcala()
        {
            VideoFileReader reader = new VideoFileReader();
            IWMPMedia saniye = axWindowsMediaPlayer1.newMedia(ofdVideoYukle.FileName);

            videoFrameList.Clear();
          
            reader.Open(ofdVideoYukle.FileName);
            this._TotalFrameCount = (int)saniye.duration * reader.FrameRate;
            this._VideoFPS = reader.FrameRate;         
            progressBar1.Maximum = this._TotalFrameCount;
            FrameSayisi = _TotalFrameCount;
            for (int i = 0; i < this._TotalFrameCount; i++)
            {
                Bitmap videoFrame = reader.ReadVideoFrame();
                string yol = "C://Users//BegovicTeam//Desktop//video sıkıştırma//frameparcalanmis//" + "image" + i + ".mpeg";
                convertMpeg(videoFrame, yol);
                progressBar1.Value += 1;
            }
            reader.Close();
            reader.Dispose();

            birlestir();

            frameParcala.Abort();
        }
        private void btnFramelereAyir_Click(object sender, EventArgs e)
        {
             frameParcala = new Thread(new ThreadStart(parcala));
             frameParcala.Start();      
        }//void
        public void  convertMpeg(Bitmap img,string yol)
        {
            stretchX = 1.0;
            stretchY = 1.0;
            string fileName = yol;
            img.Save(yol, ImageFormat.Jpeg);
            ImageFunctions imgF = new ImageFunctions();

            imgF.setImage(img);



            FileInfo info = new FileInfo(fileName);
            inputFileLength = info.Length;

            // Frame  Boyutlarını alıyoruz
            Size imgSize = imgF.getBMPSize();
            int xFill = 0;
            int yFill = 0;
            int xRem, yRem;

            xRem = imgSize.Width % 16;
            yRem = imgSize.Height % 16;

            if (xRem != 0)
                xFill = 16 - xRem;

            if (yRem != 0)
                yFill = 16 - yRem;

            /// MPEG formatına dönüştürme yapıyoruz       
            WriteMPEGSequence(img,yol);
            videoGenislik = img.Width;
            videoYukseklik = img.Height;   
            send(img);
        }
        public void send(Bitmap img1)
        {
            videoFrameList.Add(img1);
            //img1.Save(yol, ImageFormat.JpegDi
        }
        public void birlestir()
        {

            string FileName = "C://Users//BegovicTeam//Desktop//video sıkıştırma//frameBirlestir//birlesmis"+uzanti2;
            int width = 100;
            int height = 100;

            width = videoGenislik;
            height = videoYukseklik;

            VideoFileWriter writer = new VideoFileWriter();
            writer.Open(FileName, width, height, this._VideoFPS, VideoCodec.MPEG4);

            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            DirectoryInfo dir = new DirectoryInfo(fbdFramePath.SelectedPath + "\\");

            progressBar1.Maximum = FrameSayisi;

            progressBar1.Value = 0;
            foreach (Bitmap frame in videoFrameList)
            {
                image = frame;
                writer.WriteVideoFrame(image);
               
                progressBar1.Value += 1;
            }
            writer.Close();
            writer.Dispose();


        }

    }//class
}//namespace
