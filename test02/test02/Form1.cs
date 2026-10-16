using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QuanLyThietBi
{
    public partial class frmDeviceManager : Form
    {
        private BindingList<Device> devices = new BindingList<Device>();
        private BindingSource bsDevices = new BindingSource();
        private int nextId = 1;

        public frmDeviceManager()
        {
            InitializeComponent();

            CauHinhGiaoDien();
            GanSuKien();

            // Không gọi ThemDuLieuMau()
            // Khi mở chương trình, bảng dữ liệu sẽ trống.
        }

        private void CauHinhGiaoDien()
        {
            Text = "Quản lý danh mục thiết bị điện tử";
            StartPosition = FormStartPosition.CenterScreen;

            txtDeviceId.ReadOnly = true;

            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.Items.Clear();
            cboCategory.Items.AddRange(new object[]
            {
                "Laptop",
                "Điện thoại",
                "Máy tính bảng",
                "Phụ kiện",
                "Khác"
            });

            numQuantity.Minimum = 0;
            numQuantity.Maximum = 100000;
            numQuantity.Value = 1;

            dgvDevices.AutoGenerateColumns = false;
            dgvDevices.AllowUserToAddRows = false;
            dgvDevices.ReadOnly = true;
            dgvDevices.MultiSelect = false;
            dgvDevices.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDevices.RowHeadersVisible = false;
            dgvDevices.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // Tạo các cột bằng code.
            dgvDevices.Columns.Clear();

            ThemCot("Id", "Mã thiết bị", 70);
            ThemCot("Name", "Tên thiết bị", 180);
            ThemCot("Category", "Danh mục", 120);

            DataGridViewTextBoxColumn colPrice =
                new DataGridViewTextBoxColumn();

            colPrice.Name = "colPrice";
            colPrice.DataPropertyName = "Price";
            colPrice.HeaderText = "Đơn giá (VNĐ)";
            colPrice.FillWeight = 130;
            colPrice.DefaultCellStyle.Format = "N0";

            dgvDevices.Columns.Add(colPrice);

            ThemCot("Quantity", "Số lượng", 80);

            bsDevices.DataSource = devices;
            dgvDevices.DataSource = bsDevices;

            errorProvider1.ContainerControl = this;

            LamMoi();
        }

        private void ThemCot(
            string propertyName, string header, float weight)
        {
            DataGridViewTextBoxColumn col =
                new DataGridViewTextBoxColumn();

            col.Name = "col" + propertyName;
            col.DataPropertyName = propertyName;
            col.HeaderText = header;
            col.FillWeight = weight;

            dgvDevices.Columns.Add(col);
        }

        private void GanSuKien()
        {
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;

            dgvDevices.SelectionChanged +=
                dgvDevices_SelectionChanged;
        }

        private bool KiemTraDuLieu(out decimal price)
        {
            errorProvider1.Clear();
            price = 0;
            bool hopLe = true;

            if (string.IsNullOrWhiteSpace(txtDeviceName.Text))
            {
                errorProvider1.SetError(
                    txtDeviceName,
                    "Vui lòng nhập tên thiết bị.");

                hopLe = false;
            }

            if (cboCategory.SelectedIndex < 0)
            {
                errorProvider1.SetError(
                    cboCategory,
                    "Vui lòng chọn danh mục.");

                hopLe = false;
            }

            string s = txtPrice.Text.Trim();

            bool docDuoc = decimal.TryParse(
                s,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out price);

            if (!docDuoc)
            {
                docDuoc = decimal.TryParse(
                    s,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out price);
            }

            if (!docDuoc || price < 0)
            {
                errorProvider1.SetError(
                    txtPrice,
                    "Đơn giá phải là số không âm.");

                hopLe = false;
            }

            return hopLe;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            decimal price;

            if (!KiemTraDuLieu(out price))
                return;

            Device d = new Device
            {
                Id = nextId++,
                Name = txtDeviceName.Text.Trim(),
                Category = cboCategory.Text,
                Price = price,
                Quantity = (int)numQuantity.Value
            };

            devices.Add(d);

            bsDevices.Position = devices.Count - 1;

            MessageBox.Show(
                "Thêm thiết bị thành công.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LamMoi();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvDevices.CurrentRow == null ||
                dgvDevices.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn thiết bị cần cập nhật.");
                return;
            }

            decimal price;

            if (!KiemTraDuLieu(out price))
                return;

            Device d =
                dgvDevices.CurrentRow.DataBoundItem as Device;

            if (d == null)
                return;

            d.Name = txtDeviceName.Text.Trim();
            d.Category = cboCategory.Text;
            d.Price = price;
            d.Quantity = (int)numQuantity.Value;

            bsDevices.ResetCurrentItem();

            MessageBox.Show("Cập nhật thiết bị thành công.");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDevices.CurrentRow == null ||
                dgvDevices.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn thiết bị cần xóa.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa thiết bị này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Device d =
                    dgvDevices.CurrentRow.DataBoundItem as Device;

                if (d != null)
                    devices.Remove(d);

                LamMoi();

                MessageBox.Show("Đã xóa thiết bị.");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void dgvDevices_SelectionChanged(
            object sender, EventArgs e)
        {
            if (dgvDevices.CurrentRow == null ||
                dgvDevices.CurrentRow.DataBoundItem == null)
                return;

            Device d =
                dgvDevices.CurrentRow.DataBoundItem as Device;

            if (d == null)
                return;

            txtDeviceId.Text = d.Id.ToString();
            txtDeviceName.Text = d.Name;
            cboCategory.SelectedItem = d.Category;

            txtPrice.Text = d.Price.ToString(
                "0.##", CultureInfo.CurrentCulture);

            numQuantity.Value = Math.Max(
                numQuantity.Minimum,
                Math.Min(numQuantity.Maximum, d.Quantity));
        }

        private void LamMoi()
        {
            errorProvider1.Clear();

            txtDeviceId.Clear();
            txtDeviceName.Clear();
            txtPrice.Clear();

            cboCategory.SelectedIndex = -1;
            numQuantity.Value = 1;

            dgvDevices.ClearSelection();
            dgvDevices.CurrentCell = null;
        }
    }
}