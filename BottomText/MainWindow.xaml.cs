using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace BottomText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DiscordSocketClient Client { get; } = new();
        private bool IsClientReady { get; set; } = false;

        private DispatcherTimer MyTimer { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            Client.Ready += Client_Ready;
            Client.Log += Client_Log;
            Client.JoinedGuild += Client_JoinedGuild;
            try
            {
                TokenBox.Password = File.ReadAllText("token.txt");
            }
            catch (Exception )
            {
                //ignore
            }
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                var selected = GuildsBox.SelectedItem;
                GuildsBox.ItemsSource = Client.Guilds.Select(z => z.Name);
                GuildsBox.SelectedItem = selected;
            });
        }

        private Task Client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private Task Client_Ready()
        {
            IsClientReady = true;
            return Task.CompletedTask;
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectButton.IsEnabled = false;
            TokenBox.IsEnabled = false;

            string token = TokenBox.Password;
            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            while (!IsClientReady)
            {
                await Task.Delay(1);
            }

            await File.WriteAllTextAsync("token.txt", TokenBox.Password);

            GuildsBox.IsEnabled = true;
            GuildsBox.ItemsSource = Client.Guilds.Select(z => z.Name);

            InstructionsBlock.Text = "Select a server and a channel, then click Start to begin bottom texting";
            //ConnectButton.IsEnabled = true;
        }

        private void GuildsBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var guild = Client.Guilds.SingleOrDefault(z => z.Name == GuildsBox.SelectedItem.ToString());
                if (guild is null)
                {
                    GuildsBox.SelectedIndex = 0;
                    return;
                }
                ChannelBox.ItemsSource = guild.TextChannels;
                ChannelBox.IsEnabled = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            try
            {
                await Client.LogoutAsync();
                await Client.StopAsync();
            }
            catch
            {
                //ignore
            }
        }

        private string BottomText => string.IsNullOrWhiteSpace(BottomTextBox.Text) ? "<Enter bottom text>" : BottomTextBox.Text;
        private RestUserMessage? CurrentMessage { get; set; }


        private async void ButtomButton_OnClick(object sender, RoutedEventArgs e)
        {
            TokenBox.IsEnabled = false;
            ConnectButton.IsEnabled = false;
            GuildsBox.IsEnabled = false;
            ChannelBox.IsEnabled = false;
            ButtomButton.IsEnabled = false;

            SocketTextChannel channel = (SocketTextChannel)ChannelBox.SelectedItem;

            CurrentMessage = await channel.SendMessageAsync(BottomText);
            UpdateBar.IsIndeterminate = false;
            UpdateBar.Minimum = 0;
            UpdateBar.Maximum = 100;
            UpdateBar.Value = 100;

            MsgBar.IsIndeterminate = false;
            MsgBar.Minimum = 0;
            MsgBar.Maximum = 100;
            MsgBar.Value = 100;

            MyTimer.Interval = TimeSpan.FromMilliseconds(15);
            MyTimer.Tick += MyTimer_Tick;

            SendMessageTimer.Interval = TimeSpan.FromMilliseconds(5);
            SendMessageTimer.Tick += SendMessageTimer_Tick;

            BottomTextBox.TextChanged += BottomTextBox_TextChanged;
            Client.MessageReceived += async message =>
            {
                if (message.Channel.Id == channel.Id)
                {
                    await Client_MessageReceived(message);
                    return;
                }

                await CurrentMessage.DeleteAsync();
                CurrentMessage = await channel.SendMessageAsync(BottomText);
            };

            InstructionsBlock.Text = "Enter any text to update the bottom text.";

        }

        private async void SendMessageTimer_Tick(object? sender, EventArgs e)
        {
            if (MsgBar.Value >= 100)
            {
                SendMessageTimer.Stop();
                if (CurrentMessage is null)
                {
                    return;
                }

                string body = CurrentMessage.Content;
                await CurrentMessage.DeleteAsync();

                SocketTextChannel channel = (SocketTextChannel)ChannelBox.SelectedItem;

                CurrentMessage = await channel.SendMessageAsync(body);
                return;
            }

            MsgBar.Value += 1;
        }

        private async void MyTimer_Tick(object? sender, EventArgs e)
        {
            if (UpdateBar.Value >= 100)
            {
                MyTimer.Stop();
                if (CurrentMessage is null)
                {
                    return;
                }
                await CurrentMessage.ModifyAsync(z => z.Content = BottomText);
                return;
            }

            UpdateBar.Value += 2;
        }

        private void BottomTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateBar.Value = 0;
            MyTimer.Start();
        }

        private DispatcherTimer SendMessageTimer { get; set; } = new();

        private async Task Client_MessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot)
            {
                return;
            }

            await Dispatcher.InvokeAsync(() =>
            {
                MsgBar.Value = 0;
                SendMessageTimer.Start();
            });


        }

        private void ChannelBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtomButton.IsEnabled = ChannelBox.SelectedIndex != -1;
        }
    }
}