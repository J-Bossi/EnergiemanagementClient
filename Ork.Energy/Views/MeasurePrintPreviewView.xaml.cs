using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Ork.Energy.Views
{
    /// <summary>
    /// Interaktionslogik für MeasurePrintPreviewViewModel.xaml
    /// </summary>
    public partial class MeasurePrintPreviewView : UserControl
    {
        public MeasurePrintPreviewView()
        {
            InitializeComponent();
        }

        private void Print_Command(object sender, RoutedEventArgs e)
        {



            PrintDialog printDialog = new PrintDialog();



            FullControl.ScrollToTop();
            PrintButton.Visibility = Visibility.Hidden;
            BackButton.Visibility = Visibility.Hidden;
            if (printDialog.ShowDialog() == true)
            {
              
                System.Printing.PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

               
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                               this.ActualHeight);

               
                this.LayoutTransform = new ScaleTransform(scale, scale);

              
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

              
                this.Measure(sz);
                this.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                printDialog.PrintVisual(this, "Aktionsplan");

              
            }
            PrintButton.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;

        }
    }
}
