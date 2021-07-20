using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using System;
using System.IO;
using System.Json;
using System.Net;
using System.Threading.Tasks;


namespace Ejercicio5
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText txtNombre, txtDomicilio, txtCorreo, txtEdad, txtSaldo, txtID;
        Button btnSave, btnSearch;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SupportActionBar.Hide();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            txtNombre = FindViewById<EditText>(Resource.Id.txtNombre);
            txtDomicilio = FindViewById<EditText>(Resource.Id.txtDomicilio);
            txtCorreo = FindViewById<EditText>(Resource.Id.txtCorreo);
            txtEdad = FindViewById<EditText>(Resource.Id.txtEdad);
            txtSaldo = FindViewById<EditText>(Resource.Id.txtSaldo);
            txtID = FindViewById<EditText>(Resource.Id.txtID);

            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSearch = FindViewById<Button>(Resource.Id.btnSearch);

            btnSave.Click += delegate
            {
                try
                {
                    var Nombre = txtNombre.Text;
                    var Domicilio = txtDomicilio.Text;
                    var Correo = txtCorreo.Text;
                    var Edad = int.Parse(txtEdad.Text);
                    var Saldo = double.Parse(txtSaldo.Text);

                    var API = "https://serviciorestnetcorevictor.azurewebsites.net/Principal/AlmacenarSQLServer?Nombre=" + Nombre 
                    + "&Domicilio=" + Domicilio + "&Correo=" + Correo + "&Edad=" + Edad + "&Saldo=" + Saldo;
                    var request = (HttpWebRequest)WebRequest.Create(API);
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseText = reader.ReadToEnd();
                    Toast.MakeText(this, responseText.ToString(), ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            btnSearch.Click += async delegate
            {
                try
                {
                    var ID = int.Parse(txtID.Text);
                    var API = "https://serviciorestnetcorevictor.azurewebsites.net/Principal/ConsultaSQLServer?ID=" + ID;
                    JsonValue json = await Datos(API);

                    var Resultados = json[0];
                    txtNombre.Text = Resultados["nombre"];
                    txtDomicilio.Text = Resultados["domicilio"];
                    txtCorreo.Text = Resultados["correo"];
                    txtEdad.Text = Resultados["edad"].ToString();
                    txtSaldo.Text = Resultados["saldo"].ToString();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                };
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async Task<JsonValue> Datos (string API)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(API));
            request.ContentType = "application/json";
            request.Method = "Get";

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (System.IO.Stream stream = response.GetResponseStream())
                {
                    var jsondoc = await Task.Run(() => JsonValue.Load(stream));
                    return jsondoc;
                }
            }
        }
    }
}