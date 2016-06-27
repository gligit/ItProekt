using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace ItProekt
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            string UID = App_Code.Class1.GetUserIDFromUserName(Label1);
            if (UID != null)
            {
                App_Code.Class1.GetAllStickersForUsedID(UID, ListBox1, Label1);
                if (ListBox1.SelectedIndex == -1)
                {
                    Image1.Visible = false;
                    PlayerInfo.Visible = false;
                    Button1.Visible = false;
                    Button2.Visible = false;
                    TextBox1.Visible = false;
                    CheckBox1.Visible = false;
                    Label3.Visible = false;
                }
                GetAllNeedStickersForUserID(UID);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "";
            Label5.Text = "";
        }
        private void GetAllNeedStickersForUserID(string UserID)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                string UserName = HttpContext.Current.User.Identity.Name;
                string c = "SELECT PlayerName,Date FROM NeedSticker WHERE UserID=@UserID";
                SqlCommand command = new SqlCommand(c, con);
                command.Parameters.AddWithValue("@UserID", UserID);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                ListBox2.Items.Clear();
                while (reader.Read())
                {
                    string PlayerName = (string)reader["PlayerName"];
                    DateTime Date = (DateTime)reader["Date"];
                    ListBox2.Items.Add(new ListItem("Име/Презиме на играчот: "+PlayerName + " Огласено на " + Date.ToString(),PlayerName));
                }
            }
            catch (Exception err)
            {
                Label1.Text = "Грешка (4)" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
    
        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBox1.SelectedIndex != -1)
            {
                string[] pl = ListBox1.SelectedItem.Value.Split(' ');
                string StickerID = pl[0];
                string Date = pl[1];
                Image1.ImageUrl = pl[3];

                PlayerInfo.Text = "Име на играчот: " + ListBox1.SelectedItem.Text +
                    "<br/>Сликичката е креирана на " + Date + " " + pl[2] +
                     "<br/>ID на сликичката " + StickerID;

                Image1.Visible = true;
                PlayerInfo.Visible = true;
                Button1.Visible = true;
                Button2.Visible = true;
                TextBox1.Visible = true;
                CheckBox1.Visible = true;
                Label3.Visible = true;
            }
            else
            {
                Image1.Visible = false;
                PlayerInfo.Visible = false;
                Button1.Visible = false;
                Button2.Visible = false;
                TextBox1.Visible = false;
                CheckBox1.Visible = false;
                Label3.Visible = false;
            }
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            if (ListBox1.SelectedIndex != -1)
            {
                int Price = 0;
                if (App_Code.Class1.CheckForNumber(TextBox1.Text, out Price, Label1))
                {
                    string UID = App_Code.Class1.GetUserIDFromUserName(Label1);
                    string StickerID = ListBox1.SelectedItem.Value.Split(' ')[0];

                    if (UID != null)
                    {
                        App_Code.Class1.PostSticker(Label1, StickerID, UID, Price, CheckBox1.Checked);
                    }
               
                }
            }
        }

        private void DeleteSticker(string UID,string StickerID,Label ErrMessage)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand command = new SqlCommand("DeleteSticker", con);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserID", UID);
                command.Parameters.AddWithValue("@StickerID", StickerID);
                command.Parameters.Add(new SqlParameter("RetValue", System.Data.SqlDbType.Int)).Direction = System.Data.ParameterDirection.ReturnValue;

                con.Open();
                command.ExecuteNonQuery();
                int retvalue = (int)command.Parameters["RetValue"].Value;

                if (retvalue == -2)
                {
                    ErrMessage.Text = "Грешка при бришење";
                }
                else
                {
                    ErrMessage.Text = "Успешно бришење";
                }
            }
            catch (Exception err)
            {
                ErrMessage.Text = "Грешка при бришење" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        protected void Button2_Click1(object sender, EventArgs e)
        {
            if (ListBox1.SelectedIndex != -1)
            {
                string UID = App_Code.Class1.GetUserIDFromUserName(Label1);
                string StickerID = ListBox1.SelectedItem.Value.Split(' ')[0];
                if (UID != null)
                {
                    DeleteSticker(UID, StickerID, Label1);
                    App_Code.Class1.GetAllStickersForUsedID(UID, ListBox1, Label1);
                    if (ListBox1.SelectedIndex == -1)
                    {
                        Image1.Visible = false;
                        PlayerInfo.Visible = false;
                        Button1.Visible = false;
                        Button2.Visible = false;
                        TextBox1.Visible = false;
                        CheckBox1.Visible = false;
                        Label3.Visible = false;
                    }
                }
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (App_Code.Class1.CheckPlayerName(TextBox2, Label5))
            {
                string UID = App_Code.Class1.GetUserIDFromUserName(Label1);
                if (UID != null)
                {
                    MakeNeedSticker(UID,TextBox2.Text,Label5);
                    GetAllNeedStickersForUserID(UID);
                }
            }
        }
        private void MakeNeedSticker(string UID,string PlayerName,Label ErrMessage)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                
                SqlCommand command = new SqlCommand("MakeNeedSticker", con);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserID", UID);
                command.Parameters.AddWithValue("@PlayerName", PlayerName);
                command.Parameters.Add(new SqlParameter("RetValue", System.Data.SqlDbType.Int)).Direction = System.Data.ParameterDirection.ReturnValue;

                con.Open();
                command.ExecuteNonQuery();
                int retvalue = (int)command.Parameters["RetValue"].Value;

                if (retvalue == -1)
                {
                    ErrMessage.Text = "Имате поставено оглас за тој играч";
                }
                else
                {
                    ErrMessage.Text = "Успешно поставување оглас";
                }
            }
            catch (Exception err)
            {
                ErrMessage.Text = "Грешка при поставување оглас" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            } 
        }

        private void DeleteNeedSticker(string UID,string PlayerName,Label ErrMessage)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                SqlCommand command = new SqlCommand("DeleteNeedSticker", con);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserID", UID);
                command.Parameters.AddWithValue("@PlayerName", PlayerName);

                con.Open();
                
                if (command.ExecuteNonQuery() > 0)
                {
                    ErrMessage.Text = "Успешно бришење оглас";
                    GetAllNeedStickersForUserID(UID);
                }
                else
                {
                    ErrMessage.Text = "Грешка при бришење оглас querry !>0";
                }
            }
            catch (Exception err)
            {
                ErrMessage.Text = "Грешка при бришење оглас" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            } 
        }
        protected void Button4_Click(object sender, EventArgs e)
        {
            if (ListBox2.SelectedIndex != -1)
            {
                string UID = App_Code.Class1.GetUserIDFromUserName(Label5);
                if(UID != null)
                {
                string PlayerName = ListBox2.SelectedItem.Value;
                DeleteNeedSticker(UID, PlayerName, Label5);
                }
            }
        }

    }
   public class Sticker
    {
        public string StickerID { get; set; }
        public string PlayerName { get; set; }
        public DateTime Date { get; set; }
        public string ImagePath { get; set; }

        public Sticker(string StickerID, string Extension, string PlayerName, DateTime Date)
        {
            this.StickerID = StickerID;
            this.PlayerName = PlayerName;
            this.Date = Date;
            ImagePath = @"~\Images\" + StickerID + Extension;
        }
        public override string ToString()
        {
            return PlayerName;
        }
        public string getData()
        {
            return StickerID + " " + Date + " " + ImagePath;
        }

    }
}