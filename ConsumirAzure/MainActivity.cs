using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Threading.Tasks;
using System.Json;
using System.Net;
using System;
using System.IO;

namespace ConsumirAzure
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText txtNombre, txtDomicilio, txtCorreo, txtEdad, txtSaldo, txtID;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            txtID = FindViewById<EditText>(Resource.Id.txtId);
            txtNombre = FindViewById<EditText>(Resource.Id.txtNombre);
            txtDomicilio = FindViewById<EditText>(Resource.Id.txtDomicilio);
            txtCorreo = FindViewById<EditText>(Resource.Id.txtCorreo);
            txtEdad = FindViewById<EditText>(Resource.Id.txtEdad);
            txtSaldo = FindViewById<EditText>(Resource.Id.txtSaldo);
            var btnAlmacenar = FindViewById<Button>(Resource.Id.btnRegistro);
            var btnBuscar = FindViewById<Button>(Resource.Id.btnBuscar);

            //ACCION DEL BOTON DE CONSULTAR
            btnBuscar.Click += async delegate
            {
                try
                {
                    int ID = int.Parse(txtID.Text);
                    var API = "http://jorgerangel2018.azurewebsites.net/api/values/ConsultarSQLServer?ID="+ID;
                    JsonValue json = await Datos(API);
                    Transform(json);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();                  
                }
            };
            btnAlmacenar.Click += delegate
            {
                try
                {
                    var Nombre = txtNombre.Text;
                    var Domicilio = txtDomicilio.Text;
                    var Correo = txtCorreo.Text;
                    var Edad = int.Parse(txtEdad.Text);
                    var Saldo = double.Parse(txtSaldo.Text);
                    var API = "http://jorgerangel2018.azurewebsites.net/api/values/AlamcenarSQLServer?Nombre=" + Nombre + 
                    "&Domicilio="+ Domicilio + "&Correo="+ Correo + "&Edad="+ Edad + "&Saldo=" + Saldo + "";
                    var request = (HttpWebRequest)WebRequest.Create(API);
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responsetext = reader.ReadToEnd();
                    Toast.MakeText(this, responsetext.ToString(), ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };
        }

        public void Transform(JsonValue json) {
            try
            {
                var Resultados = json[0];
                txtNombre.Text = Resultados["nombre"];
                txtCorreo.Text = Resultados["correo"];
                txtDomicilio.Text = Resultados["domicilio"];
                txtEdad.Text = Resultados["edad"].ToString();
                txtSaldo.Text = Resultados["salario"].ToString();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public async Task<JsonValue> Datos(string API) {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(API));
            request.ContentType = "application/json";
            request.Method = "GET";
            using (WebResponse response = await request.GetResponseAsync()) {
                using (System.IO.Stream stream = response.GetResponseStream()) {
                    var jsondoc = await Task.Run(() => JsonValue.Load(stream));
                    return jsondoc;
                }
            }
        }
    }
}