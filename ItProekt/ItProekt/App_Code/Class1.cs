using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace ItProekt.App_Code
{
    public static class Class1
    {
        public static void PostSticker(Label ErrMessage, string StickerID, string PosterID, int Price, bool AcceptsTrading)
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
                    ErrMessage.Text = "Грешка при постирање";
                }
                else if (retvalue == -1)
                {
                    ErrMessage.Text = "Таа сликичка е веќе постирана";
                }
                else
                {
                    ErrMessage.Text = "Успешно постирање";
                }
            }
            catch (Exception err)
            {
                ErrMessage.Text = "Грешка при постирање" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
        public static string GetUserIDFromUserName(Label ErrMessage)
        {
            SqlConnection con = null;
                try
                {
                    con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                    string UserName = HttpContext.Current.User.Identity.Name;

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
                    else
                    {
                        ErrMessage.Text = "Грешка, корисникот не постои(3,1)";
                        return null;
                    }

                }
                catch (Exception err)
                {
                    ErrMessage.Text = "Грешка, обидете се повторно (3)" + err.Message;
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
                return null;
        }
        public static bool CheckForNumber(string text, out int price, Label l)
        {
            if (int.TryParse(text, out price))
                return true;
            else
            {
                l.Text = "Внесете број во полето за цена";
                return false;
            }
        }
        public static bool CheckPlayerName(TextBox t,Label l)
        {
            if (!String.IsNullOrWhiteSpace(t.Text))
            {
                return true;
            }
            else
            {
                l.Text = "Внесете име и презиме на играчот";
                return false;
            }
        }
        public static void GetAllStickersForUsedID(string UserID, ListBox ListBox1, Label ErrMessage)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                string UserName = HttpContext.Current.User.Identity.Name;
                string c = "SELECT StickerID,PlayerName,Date,Extension FROM Sticker WHERE UserID=@UserID";
                SqlCommand command = new SqlCommand(c, con);
                command.Parameters.AddWithValue("@UserID", UserID);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<Sticker> stickers = new List<Sticker>();

                while (reader.Read())
                {
                    string StickerID = reader["StickerID"].ToString();
                    string PlayerName = (string)reader["PlayerName"];
                    DateTime Date = (DateTime)reader["Date"];
                    string Extension = (string)reader["Extension"];

                    stickers.Add(new Sticker(StickerID, Extension, PlayerName, Date));
                }
                ListBox1.Items.Clear();
                for (int i = 0; i < stickers.Count; i++)
                {
                    ListBox1.Items.Add(new ListItem(stickers[i].PlayerName, stickers[i].getData()));
                }

            }
            catch (Exception err)
            {
                ErrMessage.Text = "Грешка (4)" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }

        }

        public static void CreateMessageSticker(string PosterStickerID, string SenderStickerID, string PosterID,
        string SenderID, int Offer, bool Buying,Label ErrMessage)
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

                if(PosterStickerID !=null)
                command.Parameters.AddWithValue("@PosterStickerID", PosterStickerID);

                if (SenderStickerID != null)
                    command.Parameters.AddWithValue("@SenderStickerID", SenderStickerID);

                command.Parameters.AddWithValue("@PosterID", PosterID);
                command.Parameters.AddWithValue("@SenderID", SenderID);
                command.Parameters.AddWithValue("@Offer", Offer);
                command.Parameters.AddWithValue("@Buying", Buying);

                con.Open();
                command.ExecuteNonQuery();
                ErrMessage.Text = "Успешно пратена порака";
            }
            catch (Exception err)
            {
                ErrMessage.Text = "Грешка (5)" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

    }
}