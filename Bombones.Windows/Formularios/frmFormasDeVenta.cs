using Bombones.Entidades.Entidades;
using Bombones.Servicios.Intefaces;
using Bombones.Windows.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bombones.Windows.Formularios
{
    public partial class frmFormasDeVenta : Form
    {
        private readonly IServiceProvider? _serviceProvider;
        private readonly IserviciosFormaVenta _servicio;
        private List<FormaVenta> lista = null!;
        public frmFormasDeVenta(IServiceProvider? serviceProvider)
        {
            InitializeComponent();
            _servicio = serviceProvider?.GetService<IserviciosFormaVenta>()
                ?? throw new ApplicationException("Dependencias no cargadas!!!"); ;

        }

        private IserviciosFormaVenta Get_servicio()
        {
            return _servicio;
        }





        private void MostrarDatosEnGrilla()
        {
            GridHelper.LimpiarGrilla(dgvDatos);
            foreach (var forma in lista)
            {
                var r = GridHelper.ConstruirFila(dgvDatos);
                GridHelper.SetearFila(r, forma);
                GridHelper.AgregarFila(r, dgvDatos);
            }
        }

        private void frmFormasDeVenta_Load_1(object sender, EventArgs e)
        {
            try
            {

                lista = _servicio.GetLista();
                MostrarDatosEnGrilla();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void tsbNuevo_Click(object sender, EventArgs e)
        {
            frmFormaDeVentaAE frm = new frmFormaDeVentaAE() { Text = "Nueva forma de venta" };
            DialogResult dr = frm.ShowDialog(this);

            if (dr == DialogResult.Cancel)
            {
                return;
            }

            try
            {
                FormaVenta forma = frm.GetTipo();
                if (!_servicio!.Existe(forma))
                {
                    _servicio!.Guardar(forma);
                    var r = GridHelper.ConstruirFila(dgvDatos);
                    GridHelper.SetearFila(r, forma);
                    GridHelper.AgregarFila(r, dgvDatos);
                    MessageBox.Show("Registro agregado",
                                    "Mensaje",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Registro existente\nAlta denegada",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void tsbCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
