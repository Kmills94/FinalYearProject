using System;
using System.Collections.Generic;
using System.Linq;
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
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ProjectOffline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int primaryUser = 1;
        const int secondaryUser = 2;
        string usernamePara;
        string passwordPara;
        int userLoginCred;
        string myConnectionString = "Server=vesta.uclan.ac.uk;Database=kmills-ddw;Uid=kmills;Pwd=urtluly;";
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            usernameLABEL.IsTabStop = false;
            passwordLABEL.IsTabStop = false;

        }
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                usernamePara = usernameINP.Text;
                passwordPara = passwordINP.Password;
                userLoginCred = LoginValidation(usernamePara, passwordPara);
                if (userLoginCred == primaryUser)
                {
                    Root rootWindow = new Root();
                    rootWindow.Show();
                    this.Close();
                }
                else if (userLoginCred == secondaryUser)
                {
                    User userWindow = new User();
                    userWindow.Show();
                    this.Close();
                }
                else
                {
                    errorLabel.Content = "Username OR Password was entered incorrectly";
                }
            }
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            usernamePara = usernameINP.Text;
            passwordPara = passwordINP.Password;
            userLoginCred = LoginValidation(usernamePara, passwordPara);
            if (userLoginCred == primaryUser)
            {
                Root rootWindow = new Root();
                rootWindow.Show();
                this.Close();
            }
            else if (userLoginCred == secondaryUser)
            {
                User userWindow = new User();
                userWindow.Show();
                this.Close();
            }
            else
            {
                errorLabel.Content = "Username OR Password was entered incorrectly";
            }
        }
        private int LoginValidation(string username, string password)
        {
            int returnVal = 0;
            try
            {
                MySqlConnection connection = new MySqlConnection(myConnectionString);
                connection.Open();
                MySqlCommand userCheck = connection.CreateCommand();
                userCheck.CommandText = "SELECT type FROM LoginDetails WHERE username = '" + username + "' AND password = '" + password + "'";
                if (userCheck.ExecuteScalar() != null)
                {
                    MySqlDataReader reader = userCheck.ExecuteReader();
                    while (reader.Read())
                    {
                        returnVal = reader.GetInt16(0);
                    }
                    reader.Close();
                    connection.Close();
                    return returnVal;
                }
                else
                {
                    connection.Close();
                    return returnVal;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /* Use if database goes down
        private void LoginBTN_Click(object sender, RoutedEventArgs e) 
        {
            usernamePara = usernameINP.Text;
            passwordPara = passwordINP.Password;
            Login(LoginValidation(usernamePara, passwordPara));
        }
        private int LoginValidation(string username, string password)
        {

            const string rootLoginID = "root";
            const string rootLoginPASS = "root";
            const string userLoginID = "user";
            const string userLoginPASS = "user";
            if (username == rootLoginID && password == rootLoginPASS)
            {
                return primaryUser;
            }
            else if (username == userLoginID && password == userLoginPASS)
            {
                return secondaryUser;
            }
            else
                return 0;
        }
        private void Login(int permissionType)
        {
            if (permissionType == primaryUser)
            {
                Root rootWindow = new Root();
                rootWindow.Show();
                this.Close();
            }
            else if (permissionType == secondaryUser)
            {
                User userWindow = new User();
                userWindow.Show();
                this.Close();
            }
            else
            {
                errorLabel.Content = "Username OR Password was entered incorrectly";
            }
        }*/
    }
}
