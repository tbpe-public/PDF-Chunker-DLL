using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace PdfSplitterLib
{
    public static class QrReader
    {
        //Properties
        //888888888888888888888888888888888888888888888888888888888888888888888888

        //Constructors
        //888888888888888888888888888888888888888888888888888888888888888888888888

        //Methods
        //888888888888888888888888888888888888888888888888888888888888888888888888

        public static string ReadQr(Bitmap image)
        {
            LuminanceSource luminSource; //ZXing
            BinaryBitmap biBitmap;  //Bitmap class for ZXing

            //ZXing read hints used to help the QR reader
            Dictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();

            ArrayList formats = new ArrayList(); //List of possible formats for the reader.
            Reader qrReader;    //ZXing QR code reader.
            Result qrResult;    //Result of the QR code read.
            string json = string.Empty;   //json string in the QR code.

            //Set the format
            formats.Add(BarcodeFormat.QR_CODE);     //Read QR barcodes

            //Set the read hints
            hints.Add(DecodeHintType.TRY_HARDER, true); //Optimize for accracy instead of speed.
            hints.Add(DecodeHintType.POSSIBLE_FORMATS, formats); //Set formats.

            using (image)
            {
                //Get binary bitmap from source image.
                luminSource = new BitmapLuminanceSource(image);
                biBitmap = new BinaryBitmap(new GlobalHistogramBinarizer(luminSource));

                //Try to read a QR from the binary bit map
                qrReader = new QRCodeReader();
                qrResult = (Result)qrReader.decode(biBitmap, hints);

                //if the QR read returned something
                if(qrResult != null)
                {
                    //Get the result json text.
                    json = qrResult.Text.Replace("\\", ""); //remove backslashes from text.
                }
            }

            return json;

        }
    }
}
