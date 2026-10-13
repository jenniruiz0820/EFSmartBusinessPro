using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace EFSmartBusinessPro
{
    public partial class Form1 : Form
    {
        private int? productoSeleccionadoId = null;

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.button1.Click += btnGuardar_Click;
            this.button2.Click += btnActualizar_Click;
            this.dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            CargarCombos();
            CargarProductos();
            LimpiarCampos();
        }

        #region Métodos de Carga y Limpieza

        private void CargarCombos()
        {
            using (var db = new AppDbContext())
            {
                comboBox1.DataSource = db.Categorias.ToList();
                comboBox1.DisplayMember = "Nombre";
                comboBox1.ValueMember = "CategoriaId";

                comboBox2.DataSource = db.Proveedores.ToList();
                comboBox2.DisplayMember = "Nombre";
                comboBox2.ValueMember = "ProveedorId";
            }
        }

        private void CargarProductos()
        {
            using (var db = new AppDbContext())
            {
                var lista = db.Productos
                    .Select(p => new
                    {
                        p.ProductoId,
                        Nombre = p.Nombre,
                        Categoria = p.Categoria != null ? p.Categoria.Nombre : "",
                        Proveedor = p.Proveedor != null ? p.Proveedor.Nombre : "",
                        Precio = p.Precio,
                        Stock = p.Stock,
                        FechaIngreso = p.FechaIngreso,
                        Activo = p.Activo
                    })
                    .ToList();

                dataGridView1.DataSource = lista;
            }
        }

        private void LimpiarCampos()
        {
            textBox3.Clear();
            textBox2.Clear();
            textBox4.Clear();
            textBox1.Text = DateTime.Now.ToString("dd/MM/yyyy");
            checkBox1.Checked = true;

            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;

            productoSeleccionadoId = null;
        }

        #endregion

        #region Eventos de Botones

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                using (var db = new AppDbContext())
                {
                    var nuevoProducto = new Producto
                    {
                        Nombre = textBox3.Text.Trim(),
                        CategoriaId = Convert.ToInt32(comboBox1.SelectedValue),
                        ProveedorId = Convert.ToInt32(comboBox2.SelectedValue),
                        Precio = decimal.Parse(textBox2.Text),
                        Stock = int.Parse(textBox4.Text),
                        FechaIngreso = DateTime.Parse(textBox1.Text),
                        Activo = checkBox1.Checked
                    };

                    db.Productos.Add(nuevoProducto);
                    db.SaveChanges();
                }

                MessageBox.Show("Producto registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarProductos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object? sender, EventArgs e)
        {
            if (productoSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un producto de la tabla para actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos()) return;

            try
            {
                using (var db = new AppDbContext())
                {
                    var producto = db.Productos.Find(productoSeleccionadoId);
                    if (producto != null)
                    {
                        producto.Nombre = textBox3.Text.Trim();
                        producto.CategoriaId = Convert.ToInt32(comboBox1.SelectedValue);
                        producto.ProveedorId = Convert.ToInt32(comboBox2.SelectedValue);
                        producto.Precio = decimal.Parse(textBox2.Text);
                        producto.Stock = int.Parse(textBox4.Text);
                        producto.FechaIngreso = DateTime.Parse(textBox1.Text);
                        producto.Activo = checkBox1.Checked;

                        db.SaveChanges();
                        MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarProductos();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos de DataGridView

        private void dataGridView1_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                productoSeleccionadoId = Convert.ToInt32(row.Cells["ProductoId"].Value);
                textBox3.Text = row.Cells["Nombre"].Value?.ToString();
                textBox2.Text = row.Cells["Precio"].Value?.ToString();
                textBox4.Text = row.Cells["Stock"].Value?.ToString();

                if (DateTime.TryParse(row.Cells["FechaIngreso"].Value?.ToString(), out DateTime fecha))
                {
                    textBox1.Text = fecha.ToString("dd/MM/yyyy");
                }

                checkBox1.Checked = Convert.ToBoolean(row.Cells["Activo"].Value);
                comboBox1.Text = row.Cells["Categoria"].Value?.ToString();
                comboBox2.Text = row.Cells["Proveedor"].Value?.ToString();
            }
        }

        #endregion

        #region Validaciones

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("El nombre del producto es obligatorio.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(textBox2.Text, out _))
            {
                MessageBox.Show("Ingrese un precio válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(textBox4.Text, out _))
            {
                MessageBox.Show("Ingrese una cantidad de stock válida.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!DateTime.TryParse(textBox1.Text, out _))
            {
                MessageBox.Show("Ingrese una fecha válida (dd/MM/yyyy).", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        #endregion

        private void label8_Click(object? sender, EventArgs e)
        {
        }
    }
}