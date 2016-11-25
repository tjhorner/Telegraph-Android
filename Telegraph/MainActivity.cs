using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using HeyRed.MarkdownSharp;

namespace Telegraph
{
    [Activity(Label = "Telegraph", MainLauncher = true, Icon = "@drawable/launcher_telegraph")]
    public class MainActivity : Activity
    {
        private Button btn;

        private EditText titleText;
        private EditText authorText;
        private EditText postText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            titleText = FindViewById<EditText>(Resource.Id.titleText);
            authorText = FindViewById<EditText>(Resource.Id.authorText);
            postText = FindViewById<EditText>(Resource.Id.postText);

            btn = FindViewById<Button>(Resource.Id.publishButton);
            btn.Click += Btn_Click;
        }

        private async void Btn_Click(object sender, System.EventArgs e)
        {
            Markdown parser = new Markdown(new MarkdownOptions {
                AutoHyperlink = true,
                AutoNewLines = true
            });

            //SaveResponse res = await TelegraphApi.PostSave(titleText.Text, authorText.Text, parser.Transform(postText.Text));
            SaveResponse res = await TelegraphApi.PostSave(titleText.Text, authorText.Text, postText.Text);

            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraText, "http://telegra.ph/" + res.Path);
            sendIntent.SetType("text/plain");
            StartActivity(Intent.CreateChooser(sendIntent, "Share post to..."));
        }
    }
}

