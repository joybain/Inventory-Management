using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using System.IO;
using Delve;



namespace Delve
{
    /// <summary>
    /// Summary description for util.
    /// </summary>
    public class util
    {
        public static bool LogError(Exception Ex, string Message)
        {
            // To be implemented.

            return true;
        }


        public static string ReplaceEscapeChars(string str)
        {
            if (str == "")
                return str;

            str = str.Replace("'", "''");
            str = str.Replace(" ", "");

            return str;
        }
        public static bool PopulationDropDownList(DropDownList ddl, string TableName, string query, string TextField, string ValueField)
        {
            ddl.DataSource = null;
            String connectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(connectionString);
            SqlDataAdapter myAdapter = new SqlDataAdapter(query, myConnection);
            DataSet ds = new DataSet();
            myAdapter.Fill(ds, TableName);
            DataTable Dt = ds.Tables[TableName];
            if (Dt.Rows.Count == 0)
            {
                return false;
            }
            ddl.Items.Clear();
            ddl.DataSource = Dt;
            ddl.DataTextField = TextField;
            ddl.DataValueField = ValueField;
            ddl.DataBind();
            return true;
        }
        public static bool PopulationDropDownList(DropDownList ddl, string TextField, string ValueField, DataTable dt)
        {
            ddl.Items.Clear();
            ddl.DataSource = dt;
            ddl.DataTextField = TextField;
            ddl.DataValueField = ValueField;
            ddl.DataBind();
            ddl.Items.Insert(0, "");
            return true;
        }

        
        public static bool PopulateCombo(DropDownList Combo, string TableName, string TextField, string ValueField)
        {
            Combo.DataSource = null;
            String connectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(connectionString);

            string Query = "select * from " + TableName;
            Query = Query + " order by " + TextField;

            SqlDataAdapter myAdapter = new SqlDataAdapter(Query, myConnection);
            DataSet ds = new DataSet();
            myAdapter.Fill(ds, TableName);
            DataTable Dt = ds.Tables[TableName];

            if (Dt.Rows.Count == 0)
            {
                return false;
            }
            Combo.Items.Clear();
            //Combo.AppendDataBoundItems = true;
            Combo.DataSource = Dt;            
            Combo.DataTextField = TextField;
            Combo.DataValueField = ValueField;
            Combo.DataBind();
            //Combo.DisplayMember = TextField;
            //Combo.ValueMember = ValueField;
            //Combo.SelectedValue = DefaultValue;

            return true;
        }
        public static bool PopulateCombo1(DropDownList Combo, string TableName, string TextField, string ValueField)
        {
            Combo.DataSource = null;
            String connectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(connectionString);

            string Query = "select * from " + TableName +" where year_flag='O' ";
            Query = Query + "order by " + TextField;

            SqlDataAdapter myAdapter = new SqlDataAdapter(Query, myConnection);
            DataSet ds = new DataSet();
            myAdapter.Fill(ds, TableName);
            DataTable Dt = ds.Tables[TableName];

            if (Dt.Rows.Count == 0)
            {
                return false;
            }
            Combo.Items.Clear();
            //Combo.AppendDataBoundItems = true;
            Combo.DataSource = Dt;
            Combo.DataTextField = TextField;
            Combo.DataValueField = ValueField;
            Combo.DataBind();
            //Combo.DisplayMember = TextField;
            //Combo.ValueMember = ValueField;
            //Combo.SelectedValue = DefaultValue;

            return true;
        }
        public static string ConvertBytesToString(byte[] bytes)
        {
            string output = String.Empty;
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                output = reader.ReadToEnd();
            }
            return output;
        }
        public static byte[] ConvertStringToBytes(string input)
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(input);
                writer.Flush();
            }
            return stream.ToArray();
        }
        public static byte[] ConvertImageToByteArray(System.Drawing.Image imageToConvert,
                                       System.Drawing.Imaging.ImageFormat formatOfImage)
        {
            byte[] Ret;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imageToConvert.Save(ms, formatOfImage);
                    Ret = ms.ToArray();
                }
            }
            catch (Exception) { throw; }
            return Ret;
        }
        public static System.Drawing.Bitmap BytesToBitmap(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                System.Drawing.Bitmap img = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms);
                return img;
            }
        }
        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms;
            System.Drawing.Image returnImage;
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("~/img/noimage.jpg"), FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] bt = br.ReadBytes((int)fs.Length);
            if (byteArrayIn.Length == 0)
            {
                byteArrayIn = bt;
            }
            ms = new MemoryStream(byteArrayIn);
            returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }
        public enum ResizeOptions
        {
            // Use fixed width & height without keeping the proportions
            ExactWidthAndHeight,

            // Use maximum width (as defined) and keeping the proportions
            MaxWidth,

            // Use maximum height (as defined) and keeping the proportions
            MaxHeight,

            // Use maximum width or height (the biggest) and keeping the proportions
            MaxWidthAndHeight
        }
        public static System.Drawing.Bitmap DoResize(System.Drawing.Bitmap originalImg, int widthInPixels, int heightInPixels)
        {
            System.Drawing.Bitmap bitmap;
            try
            {
                bitmap = new System.Drawing.Bitmap(widthInPixels, heightInPixels);
                using (System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // Quality properties
                    graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    graphic.DrawImage(originalImg, 0, 0, widthInPixels, heightInPixels);
                    return bitmap;
                }
            }
            finally
            {
                if (originalImg != null)
                {
                    originalImg.Dispose();
                }
            }
        }
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Bitmap image, int width, int height, ResizeOptions resizeOptions)
        {
            float f_width;
            float f_height;
            float dim;
            switch (resizeOptions)
            {
                case ResizeOptions.ExactWidthAndHeight:
                    return DoResize(image, width, height);

                case ResizeOptions.MaxHeight:
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_height <= height)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;
                    width = (int)((float)(height) * dim);
                    return DoResize(image, width, height);

                case ResizeOptions.MaxWidth:
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_width <= width)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;
                    height = (int)((float)(width) / dim);
                    return DoResize(image, width, height);

                case ResizeOptions.MaxWidthAndHeight:
                    int tmpHeight = height;
                    int tmpWidth = width;
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_width <= width && f_height <= height)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;

                    // Check if the width is ok
                    if (f_width < width)
                        width = (int)f_width;
                    height = (int)((float)(width) / dim);
                    // The width is too width
                    if (height > tmpHeight)
                    {
                        if (f_height < tmpHeight)
                            height = (int)f_height;
                        else
                            height = tmpHeight;
                        width = (int)((float)(height) * dim);
                    }
                    return DoResize(image, width, height);
                default:
                    return image;
            }
        }
    }
}