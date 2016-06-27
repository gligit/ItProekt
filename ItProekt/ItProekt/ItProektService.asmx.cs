using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Runtime.Serialization;


namespace ItProekt
{
    /// <summary>
    /// Summary description for ItProektService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ItProektService : System.Web.Services.WebService
    {
        private string GetUserIDFromUserName(string UserName)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("GetUserIDFromUserName", con);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserName", UserName);
                command.Parameters.Add("@UserID", System.Data.SqlDbType.UniqueIdentifier).Direction = System.Data.ParameterDirection.Output;

                con.Open();
                command.ExecuteNonQuery();

                if (command.Parameters["@UserID"].Value != System.DBNull.Value)
                {
                    string uid = command.Parameters["@UserID"].Value.ToString();
                    return uid;
                }
            }
            return null;
        }
        private List<MySticker> GetAllStickersForUsedID(string UserID)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                string c = "SELECT StickerID,PlayerName,Date FROM Sticker WHERE UserID=@UserID";
                SqlCommand command = new SqlCommand(c, con);
                command.Parameters.AddWithValue("@UserID", UserID);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<MySticker> stickers = new List<MySticker>();

                while (reader.Read())
                {
                    string StickerID = reader["StickerID"].ToString();
                    string PlayerName = reader["PlayerName"].ToString();
                    string Date = reader["Date"].ToString();
                    stickers.Add(new MySticker(StickerID, PlayerName, Date));
                }
                reader.Close();

                return stickers;
            }
            return null;
        }
        private string PostStickerFromID(string StickerID, string PosterID, int Price, bool AcceptsTrading)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand command = new SqlCommand("MakePostSticker", con);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@StickerID", StickerID);
                command.Parameters.AddWithValue("@PosterID", PosterID);
                command.Parameters.AddWithValue("@Price", Price);
                command.Parameters.Add(new SqlParameter("RetValue", System.Data.SqlDbType.Int)).Direction = System.Data.ParameterDirection.ReturnValue;

                if (AcceptsTrading)
                {
                    command.Parameters.AddWithValue("@AcceptsTrading", 1);
                }
                else
                {
                    command.Parameters.AddWithValue("@AcceptsTrading", 0);
                }
                con.Open();
                command.ExecuteNonQuery();
                int retvalue = (int)command.Parameters["RetValue"].Value;

                if (retvalue == -2)
                {
                    return "Грешка при постирање";
                }
                else if (retvalue == -1)
                {
                    return "Таа сликичка е веќе постирана";
                }
                else
                {
                    return "Успешно постирање";
                }
            }
            catch (Exception err)
            {
                return "Грешка при постирање" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }

        }
        private string MakeNeedSticker(string UID, string PlayerName)
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
                    return "Имате поставено оглас за тој играч";
                }
                else
                {
                    return "Успешно поставување оглас";
                }
            }
            catch (Exception err)
            {
                return "Грешка при поставување оглас" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
        private List<PostedSticker> SearchPosted(string LookingFor)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                bool should_filter = false;
                if (!string.IsNullOrWhiteSpace(LookingFor))
                {
                    LookingFor = LookingFor.ToLower();
                    should_filter = true;
                }

                string c = "SELECT PostSticker.PosterID,PostSticker.StickerID ,PostSticker.Price, PostSticker.AcceptsTrading, PostSticker.Date, Sticker.PlayerName, Sticker.Extension, aspnet_Users.UserName FROM PostSticker INNER JOIN Sticker ON PostSticker.StickerID=Sticker.StickerID INNER JOIN aspnet_Users ON aspnet_Users.UserID=Sticker.UserID";
                SqlCommand command = new SqlCommand(c, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<PostedSticker> stickers = new List<PostedSticker>();

                while (reader.Read())
                {
                    bool found = false;
                    if (!should_filter)
                    {
                        found = true;
                    }
                    else
                    {
                        string PlayerName = reader["PlayerName"].ToString().ToLower();
                        int l = LookingFor.Length;

                        if (PlayerName.Contains(LookingFor))
                            found = true;
                        else if (PlayerName.Contains(LookingFor.Substring(0, l / 2)))
                            found = true;
                        else if (PlayerName.Contains(LookingFor.Substring(l / 2, l / 2)))
                            found = true;
                        else
                            found = false;
                    }

                    if (found)
                    {
                        stickers.Add(new PostedSticker(reader["StickerID"].ToString(),
                        reader["PlayerName"].ToString(), reader["Date"].ToString(),
                       reader["UserName"].ToString(), (int)reader["Price"],
                       (bool)reader["AcceptsTrading"]));
                    }
                }
                reader.Close();
                return stickers;
            }
            return null;
        }
        private List<NeedSticker> SearchNeeded(string LookingFor)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                bool should_filter = false;
                if (!string.IsNullOrWhiteSpace(LookingFor))
                {
                    LookingFor = LookingFor.ToLower();
                    should_filter = true;
                }

                string c = "SELECT aspnet_Users.UserName,NeedSticker.UserID,NeedSticker.PlayerName,NeedSticker.Date FROM NeedSticker INNER JOIN aspnet_Users ON NeedSticker.UserID = aspnet_Users.UserID";
                SqlCommand command = new SqlCommand(c, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<NeedSticker> stickers = new List<NeedSticker>();

                while (reader.Read())
                {
                    bool found = false;
                    if (!should_filter)
                    {
                        found = true;
                    }
                    else
                    {
                        string PlayerName = reader["PlayerName"].ToString().ToLower();
                        int l = LookingFor.Length;

                        if (PlayerName.Contains(LookingFor))
                            found = true;
                        else if (PlayerName.Contains(LookingFor.Substring(0, l / 2)))
                            found = true;
                        else if (PlayerName.Contains(LookingFor.Substring(l / 2, l / 2)))
                            found = true;
                        else
                            found = false;
                    }

                    if (found)
                    {
                        stickers.Add(new NeedSticker(reader["UserName"].ToString(), reader["PlayerName"].ToString(), reader["Date"].ToString()));
                    }
                }
                return stickers;
            }
            return null;
        }
        private string CreateMessageSticker(string PosterStickerID, string SenderStickerID, string PosterID,
string SenderID, int Offer, bool Buying)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                string insert;
                if (SenderStickerID == null)
                {
                    insert = "INSERT INTO MessageSticker (PosterStickerID,PosterUserID,SenderUserID,Offer,IsSenderBuying) " +
"VALUES(@PosterStickerID,@PosterID,@SenderID,@Offer,@Buying)";
                }
                else if (PosterStickerID == null)
                {
                    insert = "INSERT INTO MessageSticker (SenderStickerID,PosterUserID,SenderUserID,Offer,IsSenderBuying) " +
"VALUES(@SenderStickerID,@PosterID,@SenderID,@Offer,@Buying)";

                }
                else
                {
                    insert = "INSERT INTO MessageSticker (PosterStickerID,SenderStickerID,PosterUserID,SenderUserID,Offer,IsSenderBuying) " +
                    "VALUES(@PosterStickerID,@SenderStickerID,@PosterID,@SenderID,@Offer,@Buying)";
                }
                SqlCommand command = new SqlCommand(insert, con);

                if (PosterStickerID != null)
                    command.Parameters.AddWithValue("@PosterStickerID", PosterStickerID);

                if (SenderStickerID != null)
                    command.Parameters.AddWithValue("@SenderStickerID", SenderStickerID);

                command.Parameters.AddWithValue("@PosterID", PosterID);
                command.Parameters.AddWithValue("@SenderID", SenderID);
                command.Parameters.AddWithValue("@Offer", Offer);
                command.Parameters.AddWithValue("@Buying", Buying);

                con.Open();
                command.ExecuteNonQuery();
                return "Успешно пратена порака";
            }
            catch (Exception err)
            {
                return "Грешка (5)" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
        private List<Message> GetAllMessages(string UserID)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                string select = "SELECT MessageSticker.MessageID,MessageSticker.PosterStickerID,MessageSticker.SenderStickerID,MessageSticker.SenderUserID,MessageSticker.Offer,MessageSticker.Date,MessageSticker.IsSenderBuying,aspnet_Users.UserName FROM MessageSticker INNER JOIN aspnet_Users ON  MessageSticker.SenderUserID=aspnet_Users.UserId WHERE PosterUserID=@UserID ORDER BY MessageSticker.Date DESC";

                SqlCommand command = new SqlCommand(select, con);
                command.Parameters.AddWithValue("@UserID", UserID);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<Message> Messages = new List<Message>();
                while (reader.Read())
                {
                    Messages.Add(new Message(reader["PosterStickerID"].ToString(), reader["SenderStickerID"].ToString(), reader["UserName"].ToString(), (int)reader["Offer"], reader["Date"].ToString(), (bool)reader["IsSenderBuying"], reader["MessageID"].ToString()));
                }
                return Messages;
            }
        }
        private bool SenderIsSelling(Message message, string ThisUserID, string SenderUserID)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
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
                ThisUserCash -= message.Offer;
                SenderCash += message.Offer;

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
                command.Parameters.AddWithValue("@StickerID", message.SenderStickerID);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }
        private string DeleteAllMessagesByStickerID(string StickerID)
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
                return "Успешно";
            }
            catch (Exception err)
            {
                return "Грешка, обидете се повторно (1234) " + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
        private bool SenderIsBuying(Message message, string ThisUserID, string SenderUserID)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
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

                ThisUserCash += message.Offer;
                SenderCash -= message.Offer;

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
                command.Parameters.AddWithValue("@StickerID", message.PosterStickerID);
                command.ExecuteNonQuery();

                //transfer sticker to poster if sender offered one
                if (!string.IsNullOrWhiteSpace(message.SenderStickerID))
                {
                    update = "UPDATE Sticker SET UserID=@ThisUserID WHERE StickerID=@StickerID";
                    command.CommandText = update;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ThisUserID", ThisUserID);
                    command.Parameters.AddWithValue("@StickerID", message.SenderStickerID);
                    command.ExecuteNonQuery();
                }

                //delete post from PostSticker
                string delete = "DELETE FROM PostSticker WHERE StickerID=@PosterStickerID AND PosterID=@PosterID";
                command.CommandText = delete;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@PosterStickerID", message.PosterStickerID);
                command.Parameters.AddWithValue("@PosterID", ThisUserID);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }
        private bool DeleteSingleMessageByMessageID(Message message)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                string delete = "DELETE FROM MessageSticker WHERE MessageID=@MessageID";
                SqlCommand command = new SqlCommand(delete, con);
                command.Parameters.AddWithValue("@MessageID", message.MessageID);
                con.Open();
                command.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        [WebMethod]
        public List<MySticker> GetMyStickers(string UserName, string Password)
        {
            if (Membership.ValidateUser(UserName, Password))
            {
                string UserID = GetUserIDFromUserName(UserName);
                if (UserID != null)
                    return GetAllStickersForUsedID(UserID);
            }
            return null;
        }
        [WebMethod]
        public string PostSticker(string UserName, string Password, string StickerID, int Price, bool AcceptsTrading)
        {
            if (Membership.ValidateUser(UserName, Password))
            {
                string UserID = GetUserIDFromUserName(UserName);
                if (UserID != null)
                    return PostStickerFromID(StickerID, UserID, Price, AcceptsTrading);
            }
            return null;
        }
        [WebMethod]
        public string PostNeededSticker(string UserName, string Password, string PlayerName)
        {
            if (Membership.ValidateUser(UserName, Password))
            {
                string UserID = GetUserIDFromUserName(UserName);
                if (UserID != null)
                    return MakeNeedSticker(UserID, PlayerName);
            }
            return null;
        }
        [WebMethod]
        public List<PostedSticker> SearchPostedStickers(string SearchForPlayer = null)
        {
            return SearchPosted(SearchForPlayer);
        }
        [WebMethod]
        public List<NeedSticker> SearchNeededStickers(string SearchForPlayer = null)
        {
            return SearchNeeded(SearchForPlayer);
        }
        [WebMethod]
        public string SendMessageForPostedSticker(string UserName, string Password, PostedSticker ps, int Offer = 0, string MyOfferedStickerID = null)
        {
            string ThisUserID = GetUserIDFromUserName(UserName);
            string PosterUserID = GetUserIDFromUserName(ps.PosterUserName);
            if (ThisUserID != PosterUserID)
            {
                if (!ps.AcceptsTrading)
                {
                    if (Offer == 0)
                    {
                        return "Корисникот прифаќа само пари, внесете понуда";
                    }
                    else
                    {
                        return CreateMessageSticker(ps.StickerID, null, PosterUserID, ThisUserID, Offer, true);
                    }
                }
                else
                {
                    if (MyOfferedStickerID != null || Offer != 0)
                    {
                        return CreateMessageSticker(ps.StickerID, MyOfferedStickerID, PosterUserID, ThisUserID, Offer, true);
                    }
                    else
                    {
                        return "Морате да понудите цена или пак да селектирате сликичка за менување";
                    }
                }
            }
            return "Грешка";
        }
        [WebMethod]
        public string SendMessageForNeededSticker(string UserName, string Password, MySticker ms, NeedSticker ns, int Price)
        {
            string ThisUserID = GetUserIDFromUserName(UserName);
            string PosterUserID = GetUserIDFromUserName(ns.UserName);

            if (ThisUserID != PosterUserID)
            {
                return CreateMessageSticker(null, ms.StickerID, PosterUserID, ThisUserID, Price, false);
            }
            else
            {
                return "Тоа е ваш оглас";
            }
        }
        public List<Message> GetMyMessages(string UserName, string Password)
        {
            if (Membership.ValidateUser(UserName, Password))
            {
                string UserID = GetUserIDFromUserName(UserName);
                if (UserID != null)
                    return GetAllMessages(UserID);
            }
            return null;
        }
        [WebMethod]
        public string AcceptOffer(string UserName, string Password, Message m)
        {
            if (Membership.ValidateUser(UserName, Password))
            {
                //prifati
                string ThisUserID = GetUserIDFromUserName(UserName);
                string SenderUserID = GetUserIDFromUserName(m.SenderUserName);

                if (!m.IsSenderBuying)
                {
                    //sender is selling
                    if (SenderIsSelling(m, ThisUserID, SenderUserID))
                    {
                        return DeleteAllMessagesByStickerID(m.SenderStickerID);
                    }
                }
                else
                {
                    //sender is buying
                    if (SenderIsBuying(m, ThisUserID, SenderUserID))
                    {
                        DeleteAllMessagesByStickerID(m.PosterStickerID);

                        if (!string.IsNullOrWhiteSpace(m.SenderStickerID))
                            DeleteAllMessagesByStickerID(m.SenderStickerID);

                        return "Успешно";
                    }
                }
            }
            return null;
        }
        [WebMethod]
        public string RefuseOffer(string UserName, string Password, Message m)
        {
            if (Membership.ValidateUser(UserName, Password))
            {
                if (DeleteSingleMessageByMessageID(m))
                    return "Успешно";
            }
            return null;
        }
    }

    public class MySticker
    {
        public string StickerID { get; set; }
        public string PlayerName { get; set; }
        public string Date { get; set; }
        public MySticker(string StickerID, string PlayerName, string Date)
        {
            this.StickerID = StickerID;
            this.PlayerName = PlayerName;
            this.Date = Date;
        }
        public MySticker()
        {

        }
    }

    public class PostedSticker
    {
        public string StickerID { get; set; }
        public string PlayerName { get; set; }
        public string PostDate { get; set; }
        public string PosterUserName { get; set; }
        public int Price { get; set; }
        public bool AcceptsTrading { get; set; }

        public PostedSticker(string StickerID, string PlayerName, string PostDate, string PosterUserName,
            int Price, bool AcceptsTrading)
        {
            this.StickerID = StickerID;
            this.PlayerName = PlayerName;
            this.PostDate = PostDate;
            this.PosterUserName = PosterUserName;
            this.Price = Price;
            this.AcceptsTrading = AcceptsTrading;
        }
        public PostedSticker()
        {
        }
    }

    public class NeedSticker
    {
        public string UserName { get; set; }
        public string PlayerName { get; set; }
        public string Date { get; set; }

        public NeedSticker(string UserName, string PlayerName, string Date)
        {
            this.UserName = UserName;
            this.PlayerName = PlayerName;
            this.Date = Date;
        }
        public NeedSticker()
        {
        }
    }

    public class Message
    {
        public string PosterStickerID { get; set; }
        public string SenderStickerID { get; set; }
        public string SenderUserName { get; set; }
        public int Offer { get; set; }
        public string Date { get; set; }
        public bool IsSenderBuying { get; set; }
        public string MessageID { get; set; }

        public Message(string PosterStickerID, string SenderStickerID, string SenderUserName,
            int Offer, string Date, bool IsSenderBuying, string MessageID)
        {
            this.PosterStickerID = PosterStickerID;
            this.SenderStickerID = SenderStickerID;
            this.SenderUserName = SenderUserName;
            this.Offer = Offer;
            this.Date = Date;
            this.IsSenderBuying = IsSenderBuying;
            this.MessageID = MessageID;
        }
        public Message()
        {
        }
    }
}
