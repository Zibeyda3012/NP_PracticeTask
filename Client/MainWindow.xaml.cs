using Client.Commands;
using Microsoft.Xaml.Behaviors.Media;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Client;


public partial class MainWindow : Window, INotifyPropertyChanged
{
    private List<string> procList;
    private string text1;

    public List<string> CommandList { get; set; } = ["Start", "Kill"];

    public List<string> ProcList { get => procList; set { procList = value; OnPropertyChanged(); } }

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

    public void RefreshCommandExecute(object obj)
    {
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
                    StreamReader reader = new StreamReader(stream);

                    string json = reader.ReadToEnd();

                    List<ProcessDTO> myList = [];

                    myList = JsonSerializer.Deserialize<ProcessDTO>(json);
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

    public void RunCommandExecute(object obj)
    {

    }

    #endregion

    //-----------------------------------------------------------------------------------------------------------------------------

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}