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
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                FillList();
            }
            else
            {
                LoadMessages();
                FillList();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StatusMessage.Text = "";
        }
        private void LoadMessages()
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                string select = "SELECT MessageSticker.MessageID,MessageSticker.PosterStickerID,MessageSticker.SenderStickerID,MessageSticker.SenderUserID,MessageSticker.Offer,MessageSticker.Date,MessageSticker.IsSenderBuying,aspnet_Users.UserName FROM MessageSticker INNER JOIN aspnet_Users ON  MessageSticker.SenderUserID=aspnet_Users.UserId WHERE PosterUserID=@UserID ORDER BY MessageSticker.Date DESC";

                SqlCommand command = new SqlCommand(select, con);
                string UserID = App_Code.Class1.GetUserIDFromUserName(StatusMessage);
                command.Parameters.AddWithValue("@UserID", UserID);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                MessageSticker.PosterUserID = UserID;

                List<MessageSticker> Messages = new List<MessageSticker>();
                while (reader.Read())
                {
                    Messages.Add(new MessageSticker(reader["PosterStickerID"].ToString(), reader["SenderStickerID"].ToString(),
                        reader["SenderUserID"].ToString(), (int)reader["Offer"], reader["Date"].ToString(), (bool)reader["IsSenderBuying"], reader["UserName"].ToString(), reader["MessageID"].ToString()));
                }

                    Session["Messages"] = Messages;
            }
            catch (Exception err)
            {
                StatusMessage.Text = "Грешка, обидете се повторно (111) " + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
        private void FillList()
        {
            if (Session["Messages"] != null)
            {
                List<MessageSticker> Messages = (List<MessageSticker>)Session["Messages"];

                ListBox1.Items.Clear();

                for (int i = 0; i < Messages.Count; i++)
                {
                    ListBox1.Items.Add("Порака од " + Messages[i].SenderUserName + " на " + Messages[i].Date);
                }
            }
        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Image1.ImageUrl = "";
            Image2.ImageUrl = "";

            if (ListBox1.SelectedIndex != -1)
            {
                if (Session["Messages"] != null)
                {
                    List<MessageSticker> Messages = (List<MessageSticker>)Session["Messages"];
                    int i = ListBox1.SelectedIndex;

                    if (!Messages[i].IsSenderBuying)
                    {
                        //sender is selling

                        string PlayerName = null;
                        string Extension = null;
                        GetStickerInfo(Messages[i].SenderStickerID, ref PlayerName, ref Extension);

                        Label1.Text = "Корисникот " + Messages[i].SenderUserName + " ви ја нуди сликичката од фудбалерот " +
                            PlayerName + ". Цена = " + Messages[i].Offer;

                        string Path = @"..\Images\" + Messages[i].SenderStickerID + Extension;
                        Image2.ImageUrl = Path;

                    }
                    else
                    {
                        //sender is buying

                        string PlayerNameThisUser = null;
                        string ExtensionThisUser = null;
                        GetStickerInfo(Messages[i].PosterStickerID, ref PlayerNameThisUser, ref ExtensionThisUser);

                        Label1.Text = "Корисникот " + Messages[i].SenderUserName + " сака да ја купи вашата сликичката од фудбалерот " +
                           PlayerNameThisUser + " за " + Messages[i].Offer;

                        string Path = @"..\Images\" + Messages[i].PosterStickerID + ExtensionThisUser;
                        Image1.ImageUrl = Path;

                        if (!string.IsNullOrWhiteSpace(Messages[i].SenderStickerID))
                        {
                            string PlayerNameSender = null;
                            string ExtensionSender = null;
                            GetStickerInfo(Messages[i].SenderStickerID, ref PlayerNameSender, ref ExtensionSender);
                            Label1.Text += " + сликичката од фудбалерот " + PlayerNameSender;

                            Path = @"..\Images\" + Messages[i].SenderStickerID + ExtensionSender;
                            Image2.ImageUrl = Path;

                        }

                    }
                    Label2.Visible = true;
                    Label3.Visible = true;
                    Image1.Visible = true;
                    Image2.Visible = true;
                    Button1.Visible = true;
                    Button2.Visible = true;
             
                }
                else
                {
                    StatusMessage.Text = "Session = null";
                }
            }
            else
            {
                Label2.Visible = false;
                Label3.Visible = false;
                Button1.Visible = false;
                Button2.Visible = false;
                Image1.ImageUrl = "";
                Image2.ImageUrl = "";
                Label1.Text = "";
                StatusMessage.Text = "index -1";
            }

        }

        private void GetStickerInfo(string StickerID, ref string PlayerName, ref string Extension)
        {
            SqlConnection con = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(StickerID))
                {
                    con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    string select = "SELECT PlayerName,Extension FROM Sticker WHERE StickerID=@StickerID";
                    SqlCommand command = new SqlCommand(select, con);

                    command.Parameters.AddWithValue("@StickerID", StickerID);

                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    PlayerName = reader["PlayerName"].ToString();
                    Extension = reader["Extension"].ToString();
                }
                else
                {
                    StatusMessage.Text = StickerID + " e null ";
                }
            }
            catch (Exception err)
            {
                StatusMessage.Text = "Грешка, обидете се повторно (12) " + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //prifati
            if (ListBox1.SelectedIndex != -1)
            {
                if (Session["Messages"] != null)
                {
                    List<MessageSticker> Messages = (List<MessageSticker>)Session["Messages"];
                    int i = ListBox1.SelectedIndex;

                    if (!Messages[i].IsSenderBuying)
                    {
                        //sender is selling
                        if (SenderIsSelling(Messages[i]))
                        {
                            DeleteAllMessagesByStickerID(Messages[i].SenderStickerID);

                            LoadMessages();
                            FillList();
                        }
                    }
                    else
                    {
                        //sender is buying
                        if (SenderIsBuying(Messages[i]))
                        {
                            DeleteAllMessagesByStickerID(Messages[i].PosterStickerID);

                            if (!string.IsNullOrWhiteSpace(Messages[i].SenderStickerID))
                                DeleteAllMessagesByStickerID(Messages[i].SenderStickerID);

                            LoadMessages();
                            FillList();
                        }
                    }


                    Image1.Visible = false;
                    Image2.Visible = false;
                    Button1.Visible = false;
                    Button2.Visible = false;
                    Label2.Visible = false;
                    Label3.Visible = false;
                    Label1.Text = "";
                }
                else
                {
                    StatusMessage.Text = "Session = null";
                }
            }
            else
            {
                StatusMessage.Text = "index -1";
            }

        }

        private bool SenderIsSelling(MessageSticker Message)
        {
            SqlConnection con = null;
            try
            {
                string ThisUserID = MessageSticker.PosterUserID;
                string SenderUserID = Message.SenderUserID;

                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                //get cash
                string select = "SELECT Cash,UserId FROM aspnet_Users WHERE UserId=@PosterID OR UserId=@SenderID";
                SqlCommand command = new SqlCommand(select, con);

                command.Parameters.AddWithValue("@PosterID", ThisUserID);
                command.Parameters.AddWithValue("@SenderID", SenderUserID);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                int ThisUserCash = 0;
                int SenderCash = 0;

                while (reader.Read())
                {
                    if (reader["UserId"].ToString() == ThisUserID)
                        ThisUserCash = (int)reader["Cash"];
                    else if (reader["UserId"].ToString() == SenderUserID)
                        SenderCash = (int)reader["Cash"];
                }
                reader.Close();
                ThisUserCash -= Message.Offer;
                SenderCash += Message.Offer;

                //update cash
                string update = "UPDATE aspnet_Users SET Cash=@Cash WHERE UserId=@Userid";
                command.CommandText = update;
                command.Parameters.Clear();

                command.Parameters.AddWithValue("@Cash", ThisUserCash);
                command.Parameters.AddWithValue("@Userid", ThisUserID);
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Cash", SenderCash);
                command.Parameters.AddWithValue("@Userid", SenderUserID);
                command.ExecuteNonQuery();

                //transfer sticker to poster
                update = "UPDATE Sticker SET UserID=@ThisUserID WHERE StickerID=@StickerID";
                command.CommandText = update;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@ThisUserID", ThisUserID);
                command.Parameters.AddWithValue("@StickerID", Message.SenderStickerID);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception err)
            {
                StatusMessage.Text = "Грешка, обидете се повторно (123) " + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return false;
        }
        private bool SenderIsBuying(MessageSticker Message)
        {
            SqlConnection con = null;
            try
            {
                string ThisUserID = MessageSticker.PosterUserID;
                string SenderUserID = Message.SenderUserID;

                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                //get cash
                string select = "SELECT Cash,UserId FROM aspnet_Users WHERE UserId=@PosterID OR UserId=@SenderID";
                SqlCommand command = new SqlCommand(select, con);

                command.Parameters.AddWithValue("@PosterID", ThisUserID);
                command.Parameters.AddWithValue("@SenderID", SenderUserID);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                int ThisUserCash = 0;
                int SenderCash = 0;

                while (reader.Read())
                {
                    if (reader["UserId"].ToString() == ThisUserID)
                        ThisUserCash = (int)reader["Cash"];
                    else if (reader["UserId"].ToString() == SenderUserID)
                        SenderCash = (int)reader["Cash"];
                }
                reader.Close();

                ThisUserCash += Message.Offer;
                SenderCash -= Message.Offer;

                //update cash
                string update = "UPDATE aspnet_Users SET Cash=@Cash WHERE UserId=@Userid";
                command.CommandText = update;
                command.Parameters.Clear();

                command.Parameters.AddWithValue("@Cash", ThisUserCash);
                command.Parameters.AddWithValue("@Userid", ThisUserID);
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Cash", SenderCash);
                command.Parameters.AddWithValue("@Userid", SenderUserID);
                command.ExecuteNonQuery();

                //transfer sticker to sender
                update = "UPDATE Sticker SET UserID=@SenderID WHERE StickerID=@StickerID";
                command.CommandText = update;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@SenderID", SenderUserID);
                command.Parameters.AddWithValue("@StickerID", Message.PosterStickerID);
                command.ExecuteNonQuery();

                //transfer sticker to poster if sender offered one
                if (!string.IsNullOrWhiteSpace(Message.SenderStickerID))
                {
                    update = "UPDATE Sticker SET UserID=@ThisUserID WHERE StickerID=@StickerID";
                    command.CommandText = update;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ThisUserID", ThisUserID);
                    command.Parameters.AddWithValue("@StickerID", Message.SenderStickerID);
                    command.ExecuteNonQuery();
                }

                //delete post from PostSticker
                string delete = "DELETE FROM PostSticker WHERE StickerID=@PosterStickerID AND PosterID=@PosterID";
                command.CommandText = delete;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@PosterStickerID", Message.PosterStickerID);
                command.Parameters.AddWithValue("@PosterID", ThisUserID);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception err)
            {
                StatusMessage.Text = "Грешка, обидете се повторно (12345) " + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return false;
        }
        private void DeleteAllMessagesByStickerID(string StickerID)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                string delete = "DELETE FROM MessageSticker WHERE PosterStickerID=@StickerID OR SenderStickerID=@StickerID";
                SqlCommand command = new SqlCommand(delete, con);

                command.Parameters.AddWithValue("@StickerID", StickerID);

                con.Open();
                command.ExecuteNonQuery();
                StatusMessage.Text = "Успешно";
            }
            catch (Exception err)
            {
                StatusMessage.Text = "Грешка, обидете се повторно (1234) " + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //odbij
            if (ListBox1.SelectedIndex != -1)
            {
                if (Session["Messages"] != null)
                {
                    List<MessageSticker> Messages = (List<MessageSticker>)Session["Messages"];

                    DeleteSingleMessageByMessageID(Messages[ListBox1.SelectedIndex]);

                    Image1.Visible = false;
                    Image2.Visible = false;
                    Button1.Visible = false;
                    Button2.Visible = false;
                    Label2.Visible = false;
                    Label3.Visible = false;
                    Label1.Text = "";



                    LoadMessages();
                    FillList();
                }
                else
                {
                    StatusMessage.Text = "Session = null";
                }
            }
            else
            {
                StatusMessage.Text = "index -1";
            }
        }

        private bool DeleteSingleMessageByMessageID(MessageSticker Message)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                string delete = "DELETE FROM MessageSticker WHERE MessageID=@MessageID";
                SqlCommand command = new SqlCommand(delete, con);

                command.Parameters.AddWithValue("@MessageID", Message.MessageID);
                con.Open();
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception err)
            {
                StatusMessage.Text = "Грешка, обидете се повторно (11) " + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return false;
        }
    }
    class MessageSticker
    {
        public string PosterStickerID { get; set; }
        public string SenderStickerID { get; set; }

        public static string PosterUserID { get; set; }

        public string SenderUserID { get; set; }
        public int Offer { get; set; }
        public string Date { get; set; }
        public bool IsSenderBuying { get; set; }

        public string SenderUserName { get; set; }
        public string MessageID { get; set; }
        public MessageSticker(string PosterStickerID, string SenderStickerID, string SenderUserID,
            int Offer, string Date, bool IsSenderBuying, string SenderUserName, string MessageID)
        {
            this.PosterStickerID = PosterStickerID;
            this.SenderStickerID = SenderStickerID;
            this.SenderUserID = SenderUserID;
            this.Offer = Offer;
            this.Date = Date;
            this.IsSenderBuying = IsSenderBuying;
            this.SenderUserName = SenderUserName;
            this.MessageID = MessageID;
            if (PosterStickerID == "")
                PosterStickerID = null;
            if (SenderStickerID == "")
                SenderStickerID = null;
        }
    }
}