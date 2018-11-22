using System;
using System.Collections.ObjectModel;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE;
using Xamarin.Forms;

namespace GreenTooth
{
    public partial class MainPage : ContentPage
    {

        IAdapter adapter;
        IBluetoothLE bluetoothBLE;
        ObservableCollection<IDevice> list = new ObservableCollection<IDevice>();

        private IDevice device;
        public MainPage()
        {
            InitializeComponent();

            bluetoothBLE = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            list = new ObservableCollection<IDevice>();
            DevicesList.ItemsSource = list;

        }



        private async void searchDevice(object sender, EventArgs e)
        {
            if (bluetoothBLE.State == BluetoothState.Off)
            {
                await DisplayAlert("Atencion", "Bluetooth esta desactivado.", "OK");
                
            }
            else
            {
                list.Clear();

                adapter.ScanTimeout = 20000;
                adapter.ScanMode = ScanMode.LowLatency;//Se puede ajustar si es de bajo o alto poder, tambien consta de modo balanceado//


                adapter.DeviceDiscovered += (obj, a) =>
                {
                    if (!list.Contains(a.Device))
                        list.Add(a.Device);
                };

                await adapter.StartScanningForDevicesAsync();

            }

        }


        private async void DevicesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            device = DevicesList.SelectedItem as IDevice;

            var result = await DisplayAlert("AVISO", "Desea conectarse a este dispositivo???", "Conectar", "Cancelar");

            if (!result)
                return;

            //Stop Scanner
            await adapter.StopScanningForDevicesAsync();

            try
            {
                await adapter.ConnectToDeviceAsync(device);

                await DisplayAlert("Conectado", "Status:" + device.State, "OK");

            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        }
    }
}
