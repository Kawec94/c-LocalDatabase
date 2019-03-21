using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Data;

namespace LocalDatabaseApp
{
    public class DataBaseConnection
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Kawec\source\repos\LocalDatabaseApp\LocalDatabaseApp\Database2.mdf;Integrated Security = True");
        SqlCommand cmd;
        SqlDataReader dr;
        DataTable dt;
        SqlDataAdapter adpt;

        public bool loginUser(string login, string password)
        {
            con.Open();
            try
            {
                String syntax = "SELECT Password FROM Accounts WHERE UserName=" + "'" + login + "'";
                cmd = new SqlCommand(syntax, con);
                dr = cmd.ExecuteReader();
                dr.Read();
                string savedPasswordHash = dr[0].ToString();
                byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        throw new UnauthorizedAccessException();
                }
                MessageBox.Show("Login success");
                con.Close();
                return true;

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Błędne hasło");
                con.Close();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                MessageBox.Show("Użytkownik nie jest zarejestrowany!");
                con.Close();
                return false;
            }
        }

        public bool registerUser(string login, string password)
        {
            con.Open();

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];


            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            String syntax = "INSERT INTO Accounts VALUES('" + login + "','" + savedPasswordHash + "','" + password + "')";
            cmd = new SqlCommand(syntax, con);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Poprawnie zapisano do bazy!");
                con.Close();
                return true;
            }
            catch
            {
                cmd = new SqlCommand("SELECT Password FROM Accounts WHERE UserName=" + "'" + login + "'", con);
                dr = cmd.ExecuteReader();
                dr.Read();
                if ((dr[0].ToString()) != "")
                    MessageBox.Show("Podany nick jest zajęty!");
                con.Close();
                return false;
            }
        }
        /*
        public DataTable showData()
        {
            adpt = new SqlDataAdapter("SELECT Name, Surname, Grade, Weight FROM Grades INNER JOIN Pupils ON Grades.ID_Pupil=Pupils.ID_Pupil", con);
            dt = new DataTable();
            adpt.Fill(dt);
            return dt;
        }
        */
        public Array pupilsList()
        {
            con.Open();
            adpt = new SqlDataAdapter("SELECT CONCAT(ID_Pupil,'. ', Name,' ', Surname) FROM Pupils", con);
            dt = new DataTable();
            adpt.Fill(dt);
            string[] arrray = dt.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
            con.Close();
            return arrray;
        }

        public DataTable showDataSelected(int pupilID)
        {
            adpt = new SqlDataAdapter("SELECT Grade, Weight FROM Grades WHERE ID_Pupil="+ pupilID, con);
            dt = new DataTable();
            adpt.Fill(dt);
            con.Close();
            return dt;
        }

        public void insertData(int pupilID, int grade, int weight)
        {
            con.Open();
            cmd = new SqlCommand("INSERT INTO Grades(ID_Pupil, Grade, Weight) VALUES ("+ pupilID + ','+ grade + ','+ weight + ")", con);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public String average(int pupilID)
        {
            con.Open();
            cmd = new SqlCommand("SELECT SUM(CAST(Grade AS decimal(18,2)) * CAST(Weight AS decimal(18,2)))/SUM (CAST(Weight AS decimal(18,2))) FROM Grades WHERE ID_Pupil=" + pupilID, con);
            dr = cmd.ExecuteReader();
            dr.Read();
            String avg = dr[0].ToString();
            con.Close();
            return avg;
        }
    }
}
