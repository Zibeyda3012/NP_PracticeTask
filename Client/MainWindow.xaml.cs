using Client.Commands;
using Microsoft.Xaml.Behaviors.Media;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client;


public partial class MainWindow : Window, INotifyPropertyChanged
{
    private List<ProcessDTO> procList;
    private string text1;

    public List<string> CommandList { get; set; } = ["Start", "Kill"];

    public List<ProcessDTO> ProcList { get => procList; set { procList = value; OnPropertyChanged(); } }

    public string text { get => text1; set { text1 = value; OnPropertyChanged(); } }


    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        RefreshCommand = new RelayCommand(RefreshCommandExecute);
        RunCommand = new RelayCommand(RunCommandExecute);
    }



    #region RefreshCommand

    public ICommand RefreshCommand { get; set; }

    private void RefreshCommandExecute(object obj)
    {
        text = string.Empty;
        comBox.SelectedItem = null;

        var client = new TcpClient();

        var port = 27001;
        var ip = IPAddress.Parse("10.1.18.8");

        var ep = new IPEndPoint(ip, port);

        try
        {
            client.Connect(ep);

            if (client.Connected)
            {
                while (true)
                {

                    var stream = client.GetStream();
                    var reader = new StreamReader(stream);

                    string json = reader.ReadToEnd();

                    var str = JsonSerializer.Deserialize<ProcessDTO>(json);

                    ProcList.Add(str);

                }

            }
        }

        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }


    #endregion

    #region RunCommand

    public ICommand RunCommand { get; set; }

    private void RunCommandExecute(object obj)
    {
        var client = new TcpClient();
        var port = 27000;
        var ip = IPAddress.Parse("10.1.18.8");

        var ep = new IPEndPoint(ip, port);


        try
        {
            client.Connect(ep);

            if (client.Connected)
            {
                while (true)
                {

                    var command = new Command();
                    command.ProcessId = Convert.ToInt32(myListBox.SelectedItem.ToString().Split('.').First());
                    command.ProcessName = myListBox.SelectedItem.ToString().Split('.').Last();
                    command.Type = comBox.SelectedItem.ToString();

                    var option = new JsonSerializerOptions { WriteIndented = true };
                    var json = JsonSerializer.Serialize(command, option);

                    byte[] byteArray = Encoding.UTF8.GetBytes(json);

                    var stream = client.GetStream();
                    stream.Write(byteArray, 0, byteArray.Length);


                }


            }

        }

        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }

    #endregion

    //-----------------------------------------------------------------------------------------------------------------------------

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void myListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var listBox = sender as ListBox;

        if (listBox is not null)
            text = listBox.SelectedItem.ToString();
    }
}