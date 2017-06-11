using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Knowzy
{
	// [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CameraPage : ContentPage
	{
        Nose _nose;

        public CameraPage(Nose nose)
        {
            _nose = nose;
            InitializeComponent();

#if WINDOWS_UWP
    var inkingWrapper = (Xamarin.Forms.Platform.UWP.NativeViewWrapper)InkingContent.Content;
    var inkCanvas = (Windows.UI.Xaml.Controls.InkCanvas)inkingWrapper.NativeElement;
    inkCanvas.InkPresenter.InputDeviceTypes =
        Windows.UI.Core.CoreInputDeviceTypes.Touch |
        Windows.UI.Core.CoreInputDeviceTypes.Mouse |
        Windows.UI.Core.CoreInputDeviceTypes.Pen;

    var inkToolbarWrapper = (Xamarin.Forms.Platform.UWP.NativeViewWrapper)InkingToolbar.Content;
    var inkToolbar = (Windows.UI.Xaml.Controls.InkToolbar)inkToolbarWrapper.NativeElement;
    inkToolbar.TargetInkCanvas = inkCanvas;
#endif
        }

        private async void captureButton_Clicked(object sender, EventArgs e)
        {
            var photoService = DependencyService.Get<IPhotoService>();
            if (photoService != null)
            {
                var imageBytes = await photoService.TakePhotoAsync();
                noseImage.Source = ImageSource.FromUri(new Uri(_nose.Image)); // set source of nose image
                image.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                imageGrid.IsVisible = true; // set visibility to true

                // Invoke Cognitive Services to get predictions on the image
                productTags.Text = "Predicting...";
                productTagProbability.Text = "";
                emotionTag.Text = "";
                // Invoke the custom vision prediction api
                var customVisionTask = Task.Run(async () =>
                {
                    var client = new HttpClient();
                    // Request headers - replace this example key with your valid subscription key.
                    client.DefaultRequestHeaders.Add("Prediction-Key", "63fe389c4f96433ba807ee948e7aa98f");

                    // Prediction URL - replace this example URL with your valid prediction URL obtained after training the model.
                    string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/a2545d9c-f6e9-41d5-9807-28991bec747c/image?iterationId=2f51acdf-f96c-481c-af49-6cae71e7a2cb";
                    using (var content = new ByteArrayContent(imageBytes))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        var response = await client.PostAsync(url, content);
                        dynamic predictionResponse = await response.Content.ReadAsStringAsync()
                            .ContinueWith((readTask) => JsonConvert.DeserializeObject(readTask.Result));
                        return Tuple.Create(predictionResponse.Predictions[0].Tag, predictionResponse.Predictions[0].Probability.Value * 100);
                    }
                });
                // Invoke the Emotion API in parallel
                var emotionTask = Task.Run(async () =>
                {
                    var client = new HttpClient();
                    // Request headers - replace this example key with your valid key.
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "7af37d1e3e6048539c76274fd4c64d72");

                    // NOTE: You must use the same region in your REST call as you used to obtain your subscription keys.
                    //   For example, if you obtained your subscription keys from westcentralus, replace "westus" in the 
                    //   URI below with "westcentralus".
                    string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize";
                    using (var content = new ByteArrayContent(imageBytes))
                    {
                        // This example uses content type "application/octet-stream".
                        // The other content types you can use are "application/json" and "multipart/form-data".
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        var response = await client.PostAsync(uri, content);
                        dynamic detectionResponse = await response.Content.ReadAsStringAsync()
                            .ContinueWith((readTask) => JsonConvert.DeserializeObject(readTask.Result));
                        // See the format of the JSON response here: https://westus.dev.cognitive.microsoft.com/docs/services/5639d931ca73072154c1ce89/operations/563b31ea778daf121cc3a5fa
                        JObject scores = detectionResponse[0].scores;
                        var highestScore = scores.Properties().OrderByDescending(score => (double)((JValue)score.Value).Value)
                            .First();
                        return Tuple.Create(highestScore.Name, (double)((JValue)highestScore.Value).Value);
                    }
                });
                await Task.WhenAll(customVisionTask, emotionTask);
                // Update the UI
                productTags.Text = customVisionTask.Result.Item1;
                productTagProbability.Text = customVisionTask.Result.Item2.ToString();
                emotionTag.Text = emotionTask.Result.Item1;
            }

        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    var bounds = AbsoluteLayout.GetLayoutBounds(noseImage);
                    bounds.X += noseImage.TranslationX;
                    bounds.Y += noseImage.TranslationY;
                    AbsoluteLayout.SetLayoutBounds(noseImage, bounds);
                    noseImage.TranslationX = 0;
                    noseImage.TranslationY = 0;
                    break;

                case GestureStatus.Running:
                    noseImage.TranslationX = e.TotalX;
                    noseImage.TranslationY = e.TotalY;
                    break;
            }
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Running:
                    noseImage.Scale *= e.Scale;
                    break;
            }
        }
    }
}