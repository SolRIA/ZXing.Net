/*
* Copyright 2012 ZXing.Net authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Microsoft.Win32;
using ZXing;

using BarcodeReader = ZXing.Presentation.BarcodeReader;
using BarcodeWriter = ZXing.Presentation.BarcodeWriter;
using BarcodeWriterGeometry = ZXing.Presentation.BarcodeWriterGeometry;

namespace WPFDemo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BarcodeReader reader = new BarcodeReader();

        public int CodeMargin
        {
            get
            {
                int.TryParse(txtMargin.Text, out int margin);
                return margin;
            }
            set => txtMargin.Text = value.ToString();
        }

        public int QuietZone
        {
            get
            {
                int.TryParse(txtQuietZone.Text, out int margin);
                return margin;
            }
            set => txtQuietZone.Text = value.ToString();
        }

        public int Version
        {
            get
            {
                int.TryParse(txtVersion.Text, out int margin);
                return margin;
            }
            set => txtVersion.Text = value.ToString();
        }

        public MainWindow()
        {
            InitializeComponent();

            //init properties
            CodeMargin = 2;
            QuietZone = 5;
            Version = 17;

            foreach (var format in MultiFormatWriter.SupportedWriters)
                cmbEncoderType.Items.Add(format);
            cmbEncoderType.SelectedItem = BarcodeFormat.QR_CODE;

            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.ALPHANUMERIC);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.BYTE);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.ECI);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.FNC1_FIRST_POSITION);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.FNC1_SECOND_POSITION);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.HANZI);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.KANJI);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.NUMERIC);
            cmbMode.Items.Add(ZXing.QrCode.Internal.Mode.STRUCTURED_APPEND);
            cmbMode.SelectedItem = ZXing.QrCode.Internal.Mode.BYTE;

            cmbError.Items.Add(ZXing.QrCode.Internal.ErrorCorrectionLevel.L);
            cmbError.Items.Add(ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
            cmbError.Items.Add(ZXing.QrCode.Internal.ErrorCorrectionLevel.Q);
            cmbError.Items.Add(ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            cmbError.SelectedItem = ZXing.QrCode.Internal.ErrorCorrectionLevel.M;

            cmbRendererType.Items.Add("WriteableBitmap");
            cmbRendererType.Items.Add("XAML Geometry");
            cmbRendererType.SelectedItem = "XAML Geometry";
        }

        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "All documents (*.*)|*.*",
                FileName = txtBarcodeImageFile.Text
            };
            if (dlg.ShowDialog(this).GetValueOrDefault(false))
            {
                txtBarcodeImageFile.Text = dlg.FileName;
            }
        }

        private void BtnDecode_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var result = reader.Decode((BitmapSource)imageBarcode.Source);

            watch.Stop();
            labDuration.Content = $"{watch.ElapsedMilliseconds} ms";
            if (result != null)
            {
                txtBarcodeType.Text = $"Format: {result.BarcodeFormat}";
                //if (result.ResultMetadata != null && result.ResultMetadata.ContainsKey(ResultMetadataType.))
                //    txtBarcodeType.Text += $" - Error: {result.ResultMetadata[ResultMetadataType.ERROR_CORRECTION_LEVEL]}";
                if (result.ResultMetadata != null && result.ResultMetadata.ContainsKey(ResultMetadataType.ERROR_CORRECTION_LEVEL))
                    txtBarcodeType.Text += $" - Error: {result.ResultMetadata[ResultMetadataType.ERROR_CORRECTION_LEVEL]}";
                txtBarcodeContent.Text = result.Text;
            }
            else
            {
                txtBarcodeType.Text = "";
                txtBarcodeContent.Text = "No barcode found.";
            }
        }

        private void TxtBarcodeImageFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(txtBarcodeImageFile.Text))
                imageBarcode.Source = new BitmapImage(new Uri(txtBarcodeImageFile.Text));
        }

        private void BtnEncode_Click(object sender, RoutedEventArgs e)
        {
            imageBarcodeEncoder.Visibility = Visibility.Hidden;
            imageBarcodeEncoderGeometry.Visibility = Visibility.Hidden;
            BarcodeFormat barcodeFormat = (BarcodeFormat)cmbEncoderType.SelectedItem;
            ZXing.QrCode.Internal.Mode mode = (ZXing.QrCode.Internal.Mode)cmbMode.SelectedItem;
            ZXing.QrCode.Internal.ErrorCorrectionLevel errorCorrection = (ZXing.QrCode.Internal.ErrorCorrectionLevel)cmbError.SelectedItem;
            int? version = null;
            if (Version >= 1 && Version <= 40)
                version = Version;

            ZXing.Common.EncodingOptions options;
            if (barcodeFormat == BarcodeFormat.QR_CODE)
            {
                options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    Height = (int)((FrameworkElement)imageBarcodeEncoder.Parent).ActualHeight,
                    Width = (int)((FrameworkElement)imageBarcodeEncoder.Parent).ActualWidth,
                    Margin = CodeMargin,
                    QuietZone = QuietZone,
                    Mode = mode,
                    ErrorCorrection = errorCorrection,
                    QrVersion = version
                };
            }
            else
            {
                options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    Height = (int)((FrameworkElement)imageBarcodeEncoder.Parent).ActualHeight,
                    Width = (int)((FrameworkElement)imageBarcodeEncoder.Parent).ActualWidth,
                    Margin = CodeMargin
                };
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {

                switch (cmbRendererType.SelectedItem.ToString())
                {
                    case "WriteableBitmap":
                        {
                            var writer = new BarcodeWriter
                            {
                                Format = barcodeFormat,
                                Options = options
                            };
                            var image = writer.Write(txtBarcodeContentEncode.Text);
                            imageBarcodeEncoder.Source = image;
                            imageBarcodeEncoder.Visibility = Visibility.Visible;
                        }
                        break;
                    case "XAML Geometry":
                        {
                            var writer = new BarcodeWriterGeometry
                            {
                                Format = barcodeFormat,
                                Options = options
                            };
                            var image = writer.Write(txtBarcodeContentEncode.Text);
                            imageBarcodeEncoderGeometry.Data = image;
                            imageBarcodeEncoderGeometry.Visibility = Visibility.Visible;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            watch.Stop();
            lblContent.Content = $"Content ({watch.ElapsedMilliseconds} ms)";
        }
    }
}
