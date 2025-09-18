using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SuperStore
{
    public partial class Form1 : Form
    {
        // Use same connection string 
        private string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=SuperStoreDB;Integrated Security=True;";
        private int selectedPID = -1;

        // ensure placeholders handlers added only once
        private bool placeholdersInitialized = false;

        public Form1()
        {
            InitializeComponent();

            // wire load and DataGridView click (designer wires button clicks)
            this.Load += Form1_Load;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.CellClick += dataGridView1_CellClick;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();

            // initialize placeholders (only once)
            if (!placeholdersInitialized)
            {
                InitializePlaceholders();
                placeholdersInitialized = true;
            }

            // ensure textboxes start showing placeholders
            ResetPlaceholders();
        }

        // ---------------- Placeholder logic ----------------
        // We'll store the placeholder text in the TextBox.Tag property.
        private void InitializePlaceholders()
        {
            SetPlaceholderInitial(txtproductname, "Enter Product Name");
            SetPlaceholderInitial(txtprice, "Enter Price");
            SetPlaceholderInitial(txtMfgdate, "Enter Mfg Date");
            SetPlaceholderInitial(txtexpirydate, "Enter Expiry Date");
            SetPlaceholderInitial(txtquantity, "Enter Quantity");
            SetPlaceholderInitial(txtpacking, "Enter Packing");
        }

        private void SetPlaceholderInitial(TextBox txt, string placeholder)
        {
            txt.Tag = placeholder;
            // set initial look (actual text will be set by ResetPlaceholders)
            txt.ForeColor = Color.Gray;

            // attach generic handlers (safe to attach once)
            txt.GotFocus += TextBox_GotFocus;
            txt.LostFocus += TextBox_LostFocus;
        }

        private void ResetPlaceholders()
        {
            // set each textbox to its placeholder text and gray color
            SetPlaceholderTextDirect(txtproductname);
            SetPlaceholderTextDirect(txtprice);
            SetPlaceholderTextDirect(txtMfgdate);
            SetPlaceholderTextDirect(txtexpirydate);
            SetPlaceholderTextDirect(txtquantity);
            SetPlaceholderTextDirect(txtpacking);

            selectedPID = -1;
        }

        private void SetPlaceholderTextDirect(TextBox txt)
        {
            if (txt == null) return;
            string p = txt.Tag as string ?? "";
            txt.Text = p;
            txt.ForeColor = Color.Gray;
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null) return;
            string placeholder = txt.Tag as string ?? "";
            if (txt.Text == placeholder)
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;
            }
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null) return;
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                string placeholder = txt.Tag as string ?? "";
                txt.Text = placeholder;
                txt.ForeColor = Color.Gray;
            }
        }

        // Gets the user-entered text, returns "" if placeholder is present
        private string GetSafeText(TextBox txt)
        {
            if (txt == null) return "";
            string placeholder = txt.Tag as string ?? "";
            if (txt.ForeColor == Color.Gray || txt.Text == placeholder) return "";
            return txt.Text.Trim();
        }

        // ---------------- Database / CRUD ----------------

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT PID, ProductName, Price, MFGDate, ExpiryDate, Quantity, Packing FROM Products", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error loading data: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Add product — method name must match Designer wiring (btnadd.Click -> btnAdd_Click)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddProduct();
        }

        // In case Designer used lowercase event handler name like btnadd_Click, we provide that too
        private void btnadd_Click(object sender, EventArgs e)
        {
            AddProduct();
        }

        private void AddProduct()
        {
            string name = GetSafeText(txtproductname);
            string priceText = GetSafeText(txtprice);
            string qtyText = GetSafeText(txtquantity);
            string mfgText = GetSafeText(txtMfgdate);
            string expText = GetSafeText(txtexpirydate);
            string packing = GetSafeText(txtpacking);

            // basic validation
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter product name.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(priceText, out int price))
            {
                MessageBox.Show("Invalid Price. Please enter a valid integer value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(qtyText, out int quantity))
            {
                MessageBox.Show("Invalid Quantity. Please enter a valid integer value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DateTime.TryParse(mfgText, out DateTime mfgDate))
            {
                MessageBox.Show("Invalid Manufacturing Date. Use YYYY-MM-DD or a valid date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DateTime.TryParse(expText, out DateTime expDate))
            {
                MessageBox.Show("Invalid Expiry Date. Use YYYY-MM-DD or a valid date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Products (ProductName, Price, MFGDate, ExpiryDate, Quantity, Packing) VALUES (@name, @price, @mfg, @exp, @qty, @pack)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@mfg", mfgDate);
                    cmd.Parameters.AddWithValue("@exp", expDate);
                    cmd.Parameters.AddWithValue("@qty", quantity);
                    cmd.Parameters.AddWithValue("@pack", packing);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    con.Close();

                    if (rows > 0)
                    {
                        MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        ResetPlaceholders();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Update handlers (Designer might call btnUpdate_Click or btnupdate_Click)
        private void btnUpdate_Click(object sender, EventArgs e) => UpdateProduct();
        private void btnupdate_Click(object sender, EventArgs e) => UpdateProduct();

        private void UpdateProduct()
        {
            if (selectedPID == -1)
            {
                MessageBox.Show("Please select a product from the table to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = GetSafeText(txtproductname);
            string priceText = GetSafeText(txtprice);
            string qtyText = GetSafeText(txtquantity);
            string mfgText = GetSafeText(txtMfgdate);
            string expText = GetSafeText(txtexpirydate);
            string packing = GetSafeText(txtpacking);

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter product name.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(priceText, out int price))
            {
                MessageBox.Show("Invalid Price. Please enter a valid integer value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(qtyText, out int quantity))
            {
                MessageBox.Show("Invalid Quantity. Please enter a valid integer value.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DateTime.TryParse(mfgText, out DateTime mfgDate))
            {
                MessageBox.Show("Invalid Manufacturing Date. Use YYYY-MM-DD or a valid date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DateTime.TryParse(expText, out DateTime expDate))
            {
                MessageBox.Show("Invalid Expiry Date. Use YYYY-MM-DD or a valid date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Products SET ProductName=@name, Price=@price, MFGDate=@mfg, ExpiryDate=@exp, Quantity=@qty, Packing=@pack WHERE PID=@pid";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@pid", selectedPID);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@mfg", mfgDate);
                    cmd.Parameters.AddWithValue("@exp", expDate);
                    cmd.Parameters.AddWithValue("@qty", quantity);
                    cmd.Parameters.AddWithValue("@pack", packing);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    con.Close();

                    if (rows > 0)
                    {
                        MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        ResetPlaceholders();
                    }
                    else
                    {
                        MessageBox.Show("No product found with the selected ID to update, or no changes were made.", "Update Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Delete handlers (Designer references btnDelete_Click)
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedPID == -1)
            {
                MessageBox.Show("Please select a product from the table to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Products WHERE PID=@pid";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@pid", selectedPID);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    con.Close();

                    if (rows > 0)
                    {
                        MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        ResetPlaceholders();
                    }
                    else
                    {
                        MessageBox.Show("No product found with the selected ID to delete.", "Deletion Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Search — Designer may reference btnSearch_Click or btnsearch_Click
        private void btnSearch_Click(object sender, EventArgs e) => SearchProduct();
        private void btnsearch_Click(object sender, EventArgs e) => SearchProduct();

        private void SearchProduct()
        {
            string name = GetSafeText(txtproductname);
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter product name to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT PID, ProductName, Price, MFGDate, ExpiryDate, Quantity, Packing FROM Products WHERE ProductName LIKE @name";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@name", "%" + name + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error during search: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DataGrid row click: fill the textboxes and set selectedPID
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    if (row.Cells["PID"].Value == DBNull.Value) return;

                    selectedPID = Convert.ToInt32(row.Cells["PID"].Value);

                    // fill textboxes with black text (actual values)
                    SetTextBoxFromCell(txtproductname, row, "ProductName");
                    SetTextBoxFromCell(txtprice, row, "Price");
                    SetTextBoxFromCell(txtMfgdate, row, "MFGDate");
                    SetTextBoxFromCell(txtexpirydate, row, "ExpiryDate");
                    SetTextBoxFromCell(txtquantity, row, "Quantity");
                    SetTextBoxFromCell(txtpacking, row, "Packing");
                }
            }
            catch (Exception ex)
            {
               
                MessageBox.Show("Error selecting row: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTextBoxFromCell(TextBox txt, DataGridViewRow row, string columnName)
        {
            if (row.Cells[columnName].Value != DBNull.Value)
            {
                txt.ForeColor = Color.Black;
                txt.Text = row.Cells[columnName].Value.ToString();
            }
            else
            {
                // reset to placeholder if DB value null
                string p = txt.Tag as string ?? "";
                txt.Text = p;
                txt.ForeColor = Color.Gray;
            }
        }

       
        private void label1_Click(object sender, EventArgs e) { }
        private void txtpacking_TextChanged(object sender, EventArgs e) { }
        private void txtproductname_TextChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            
            SignIn signInForm = new SignIn();
            signInForm.Show();

           
            this.Close();
        }
    }
}
