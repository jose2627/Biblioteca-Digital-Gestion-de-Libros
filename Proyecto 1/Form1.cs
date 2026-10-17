using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto_1
{
    public partial class Form1 : Form
    {

        private readonly string cadenaConexion =
            "Server=localhost;Port=3306;Database=laboratorio_crud;" +
            "Uid=root;Pwd=;";

        private int? idSeleccionado = null;

        public Form1()
        {
            InitializeComponent();
            button1.Click -= button1_Click; button1.Click += button1_Click;   // Guardar
            button2.Click -= button2_Click; button2.Click += button2_Click;   // Consultar
            button3.Click -= button3_Click; button3.Click += button3_Click;   // Actualizar
            button4.Click -= button4_Click; button4.Click += button4_Click;   // Limpiar
            button5.Click -= button5_Click; button5.Click += button5_Click;   // Eliminar
            button6.Click -= button6_Click; button6.Click += button6_Click;   // conectar
            listBox1.SelectedIndexChanged -= listBox1_SelectedIndexChanged;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;     // lista de registros
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Mensaje("Presione conectar para cargar los registros.", false);
        }

        // El boton de conectar
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    MessageBox.Show("¡Conectado con XAMPP!", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Consultar();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("No se pudo conectar con XAMPP: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                EstadoConexion(false);
                Mensaje("No fue posible conectar con la base de datos.", true);
            }
        }

        // Utilidades 
        private void Mensaje(string texto, bool esError)
        {
            if (esError)
                label6.ForeColor = Color.Firebrick;
            else
                label6.ForeColor = Color.DarkGreen;

            label6.Text = texto;
        }

        // Nos indica con la barra lateral que el MySQL esta Conectado  o puede estar sin conexion
        private void EstadoConexion(bool conectado)
        {
            if (conectado)
            {
                label12.Text = "MySQL: Conectado";
                label12.ForeColor = Color.DarkGreen;
            }
            else
            {
                label12.Text = "MySQL: Sin conexión";
                label12.ForeColor = Color.Firebrick;
            }
        }

        private bool Validar(out string titulo, out string autor,
                             out int anio, out string genero)
        {
            titulo = textBox1.Text.Trim();
            autor = textBox4.Text.Trim();
            anio = 0;
            genero = "";

            if (titulo.Length == 0)
            {
                Mensaje("El título es obligatorio.", true);
                return false;
            }

            if (autor.Length == 0)
            {
                Mensaje("El autor es obligatorio.", true);
                return false;
            }

            if (!int.TryParse(textBox2.Text.Trim(), NumberStyles.None,
                              CultureInfo.InvariantCulture, out anio) || anio <= 0)
            {
                Mensaje("Ingrese el año de publicación como un entero mayor que cero.", true);
                return false;
            }

            if (listBox2.SelectedItem == null)
            {
                Mensaje("Seleccione un género.", true);
                return false;
            }
            genero = listBox2.SelectedItem.ToString();

            return true;
        }

        private void LimpiarFormulario()
        {
            textBox1.Clear();
            textBox4.Clear();
            textBox2.Clear();
            listBox2.ClearSelected();
            checkBox1.Checked = false;
            listBox1.ClearSelected();
            idSeleccionado = null;
        }

        private void Consultar()
        {
            try
            {
                listBox1.Items.Clear();

                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string sql = "SELECT id, titulo, autor, anio_publicacion, genero " +
                                 "FROM libros ORDER BY id;";

                    using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                    using (MySqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Libro l = new Libro();
                            l.Id = lector.GetInt32("id");
                            l.Titulo = lector.GetString("titulo");
                            l.Autor = lector.GetString("autor");
                            l.Anio = lector.GetInt32("anio_publicacion");
                            l.Genero = lector.GetString("genero");
                            listBox1.Items.Add(l);
                        }
                    }
                }
                EstadoConexion(true);
                Mensaje("Registros recuperados: " + listBox1.Items.Count + ".", false);
            }
            catch (MySqlException)
            {
                EstadoConexion(false);
                Mensaje("No fue posible conectar con la base de datos.", true);
            }
            catch (Exception ex)
            {
                Mensaje("Error inesperado: " + ex.Message, true);
            }
        }

        // El boton de consultas
        private void button2_Click(object sender, EventArgs e)
        {
            Consultar();
        }

        // El boton de guardar
        private void button1_Click(object sender, EventArgs e)
        {
            string titulo;
            string autor;
            int anio;
            string genero;

            if (!Validar(out titulo, out autor, out anio, out genero))
                return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string sql = "INSERT INTO libros (titulo, autor, anio_publicacion, genero) " +
                                 "VALUES (@titulo, @autor, @anio, @genero);";

                    using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@titulo", titulo);
                        comando.Parameters.AddWithValue("@autor", autor);
                        comando.Parameters.AddWithValue("@anio", anio);
                        comando.Parameters.AddWithValue("@genero", genero);
                        comando.ExecuteNonQuery();
                    }
                }

                Consultar();
                LimpiarFormulario();
                Mensaje("Libro guardado correctamente.", false);
            }
            catch (MySqlException)
            {
                EstadoConexion(false);
                Mensaje("No fue posible guardar el registro (revise la conexión).", true);
            }
        }
         
        // SELECCIONAR de la lista 
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Libro l = listBox1.SelectedItem as Libro;

            if (l == null)
                return;

            idSeleccionado = l.Id;
            textBox1.Text = l.Titulo;
            textBox4.Text = l.Autor;
            textBox2.Text = l.Anio.ToString();
            listBox2.SelectedItem = l.Genero;
            checkBox1.Checked = false;

            Mensaje("Registro cargado. Puede modificarlo o eliminarlo.", false);
        }

        // El boton actualizar
        private void button3_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == null)
            {
                Mensaje("Seleccione un registro para actualizar.", true);
                return;
            }

            string titulo;
            string autor;
            int anio;
            string genero;

            if (!Validar(out titulo, out autor, out anio, out genero))
                return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string sql = "UPDATE libros SET titulo = @titulo, autor = @autor, " +
                                 "anio_publicacion = @anio, genero = @genero WHERE id = @id;";

                    using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@titulo", titulo);
                        comando.Parameters.AddWithValue("@autor", autor);
                        comando.Parameters.AddWithValue("@anio", anio);
                        comando.Parameters.AddWithValue("@genero", genero);
                        comando.Parameters.AddWithValue("@id", idSeleccionado.Value);
                        comando.ExecuteNonQuery();
                    }
                }

                Consultar();
                LimpiarFormulario();
                Mensaje("Libro actualizado correctamente.", false);
            }
            catch (MySqlException)
            {
                EstadoConexion(false);
                Mensaje("No fue posible actualizar el registro (revise la conexión).", true);
            }
        }

        // El boton eliminar
        private void button5_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == null)
            {
                Mensaje("Seleccione un registro para eliminar.", true);
                return;
            }

            if (!checkBox1.Checked)
            {
                Mensaje("Marque la confirmación antes de eliminar.", true);
                return;
            }

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    string sql = "DELETE FROM libros WHERE id = @id;";

                    using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@id", idSeleccionado.Value);
                        comando.ExecuteNonQuery();
                    }
                }

                Consultar();
                LimpiarFormulario();
                Mensaje("Libro eliminado correctamente.", false);
            }
            catch (MySqlException)
            {
                EstadoConexion(false);
                Mensaje("No fue posible eliminar el registro (revise la conexión).", true);
            }
        }

        // El boton de Limpiar
        private void button4_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            Mensaje("Formulario listo para un nuevo registro.", false);
        }
    }
}