using Microsoft.Knowzy.DataProvider;
using Microsoft.Knowzy.Domain;
using Microsoft.Knowzy.Domain.Enums;
using Microsoft.Knowzy.UWP.Services;
using Microsoft.Knowzy.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Microsoft.Knowzy.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private JsonDataProvider _provider = new JsonDataProvider();

        private EditItemViewModel _editItemViewModel;

        //private InventoryDataTable _inventoryDataTable;

        private List<ChartData> _chartData;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //_inventoryDataTable = InventoryBLL.Current.GetInventory();
            DataGridInventory.ItemsSource = await _provider.GetDataAsync(); //_inventoryDataTable;

            _chartData = (from i in await _provider.GetDataAsync()
                          orderby i.Status
                          group i by i.Status into grp
                          select new ChartData { Category = grp.Key.ToString(), Value = grp.Count() }).ToList();

            RadLineSeries.ItemsSource = await _provider.GetDataAsync();
        }

        private async void NewInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            _editItemViewModel = new ViewModels.EditItemViewModel
            {
                Id = "NewUserId",
                Name = "New Name",
                Notes = "New Notes"
            };

            var editItemView = new EditItemView
            {
                EditItemViewModel = _editItemViewModel
            };

            await UserActivityService.Current.RecordInventoryUserActivity(_editItemViewModel);

            await editItemView.ShowAsync();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            var parameters = e.Parameter.ToString().Split("id=");

            if (parameters.Length <= 1) return;

            bool isNewItem = false;

            //InventoryRow inventoryRow = null;
            Product product = null;

            if (!isNewItem)
            {
                product = await _provider.GetDataByIdAsync(parameters[1]); //InventoryBLL.Current.GetInventory().FindById(parameters[1]);
            }

            isNewItem = (product == null);

            _editItemViewModel = new EditItemViewModel();

            if (!isNewItem && product != null)
            {
                _editItemViewModel.Id = product.Id;
                _editItemViewModel.Engineer = product.Engineer;
                _editItemViewModel.Name = product.Name;
                _editItemViewModel.RawMaterial = product.RawMaterial;
                _editItemViewModel.DevelopmentStartDate = product.DevelopmentStartDate;
                _editItemViewModel.ExpectedCompletionDate = product.ExpectedCompletionDate;
                _editItemViewModel.Notes = product.Notes;
                _editItemViewModel.ImageSource = product.ImageSource;
            }

            var editItemView = new EditItemView
            {
                EditItemViewModel = _editItemViewModel,
            };

            await editItemView.ShowAsync();
        }

        private void ToggleViewButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridInventory.Visibility = DataGridInventory.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            RadChart.Visibility = RadChart.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        }

        private async void EditInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedInventory = DataGridInventory.SelectedItem as Product;

            if (selectedInventory != null)
            {
                _editItemViewModel = new ViewModels.EditItemViewModel
                {
                    Id = selectedInventory.Id,
                    Engineer = selectedInventory.Engineer,
                    Name = selectedInventory.Name,
                    RawMaterial = selectedInventory.RawMaterial,
                    DevelopmentStatus = Enum.Parse<DevelopmentStatus>(selectedInventory.Status.ToString(), true),
                    DevelopmentStartDate = selectedInventory.DevelopmentStartDate,
                    ExpectedCompletionDate = selectedInventory.ExpectedCompletionDate,
                    Notes = selectedInventory.Notes,
                    ImageSource = selectedInventory.ImageSource
                };

                var editItemView = new EditItemView
                {
                    EditItemViewModel = _editItemViewModel
                };

                await UserActivityService.Current.RecordInventoryUserActivity(_editItemViewModel);

                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("nose", SelectedImage);
                SelectedImage.Visibility = Visibility.Collapsed;

                editItemView.Closing += (s, ev) => 
                {
                    SelectedImage.Visibility = Visibility.Visible;
                    ConnectedAnimation noseAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("nose");
                    if (noseAnimation != null)
                    {
                        noseAnimation.TryStart(SelectedImage);
                    }
                };

                await editItemView.ShowAsync();
            }
            else
            {
                await new MessageDialog("Please select an item to edit").ShowAsync();
            }
        }

        private void CheckBoxPWILOEnabled_Checked(object sender, RoutedEventArgs e)
        {
            App.IsPWILOEnabled = true;
        }

        private void CheckBoxPWILOEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            App.IsPWILOEnabled = false;
        }
    }
}
