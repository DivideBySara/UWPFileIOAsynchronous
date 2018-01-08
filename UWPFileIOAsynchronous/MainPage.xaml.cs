using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPFileIOAsynchronous
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CancellationTokenSource cts;
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        // *** for file i/o
        //private async void btn_Click(object sender, RoutedEventArgs e)
        //{
        //    StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        //    StorageFile file = await folder.GetFileAsync("help.txt");
        //    string text = await FileIO.ReadTextAsync(file);
        //    await Task.Delay(3000);
        //    tBlock.Text = text;
        //}

        private async void btn2_Click(object sender, RoutedEventArgs e)
        {
            // *** Display all pics in a folder
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            var files = await folder.GetFilesAsync();

            CancelPictureLoad();

            cts = new CancellationTokenSource();

            var imageAddingTasks = new List<Task<Image>>();

            foreach (var file in files)
            {
                imageAddingTasks.Add(AddImageFromFileAsync(file, cts.Token));
            }

            try
            {
                await Task.WhenAll(imageAddingTasks);
            }
            catch (OperationCanceledException)
            {
                return;
            }            

            foreach (var task in imageAddingTasks)
            {
                var image = await task;
                if (image != null) { sp.Children.Add(image); }         
            }

           

            // *** Choose a picture
            //var picker = new FileOpenPicker();
            //picker.ViewMode = PickerViewMode.Thumbnail;}
            //picker.SuggestedStartLocation = PickerLocationId.Downloads;
            //picker.FileTypeFilter.Add(".jpg");
            //picker.FileTypeFilter.Add(".png");
            //picker.FileTypeFilter.Add(".tif");
            //picker.FileTypeFilter.Add(".svg");

            //StorageFile file = await picker.PickSingleFileAsync();
            //var stream = await file.OpenReadAsync();

            //var image = new BitmapImage();
            //image.SetSource(stream);

            //picture.Source = image;
        }

        private void CancelPictureLoad()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }
        private async Task<Image> AddImageFromFileAsync(StorageFile file, CancellationToken token)
        {
            await Task.Delay(2000);

            using (var stream = await file.OpenReadAsync())
            {
                token.ThrowIfCancellationRequested();

                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);

                var image = new Image() { Width = 200, Height = 200 };
                image.Source = bitmapImage;

                return image;
            }
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            CancelPictureLoad();
        }
    }
}
