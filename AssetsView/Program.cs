using AssetsTools.NET;
using AssetsTools.NET.Extra;

using AssetsView.Winforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetsView
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var argv = Environment.GetCommandLineArgs();

#if false
            argv = new string[3];
            argv[1] = "boss_guangdaozhu_unity_7a558bb0979c3b91492bf32e05bb472f.awb";
            argv[2] = "boss_guangdaozhu_unity_7a558bb0979c3b91492bf32e05bb472f_clear_bytes.awb";
            //argv[1] = "boss_adversachevalier_01_ca1425448e706fab9efda722a066043b.awb";
            //argv[2] = "boss_adversachevalier_01_ca1425448e706fab9efda722a066043b_clear_bytes.awb";
#endif

            if (argv.Length == 1)
            {
                Application.Run(new StartScreen());
            }
            else if (argv.Length == 2)
            {
                Clear4Bytes(argv[1]);
            }
            else if(argv.Length == 3)
            {
                Clear4Bytes(argv[1], argv[2]);
            }
        }

        static void Clear4Bytes(string src, string dst = "")
        {
            var helper = new AssetsManager();
            helper.updateAfterLoad = false;
            //if (!File.Exists("classdata.tpk"))
            //{
            //    MessageBox.Show("classdata.tpk could not be found. Make sure it exists and restart.", "Assets View");
            //    Application.Exit();
            //}
            //helper.LoadClassPackage("classdata.tpk");

            if(string.IsNullOrEmpty(dst))
            {
                dst = Path.ChangeExtension(src, "_clear_bytes.awb");
            }

            if(dst.Contains('/') && !Directory.Exists(Path.GetDirectoryName(dst)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dst));
            }

            // 必须删除，否则文件内存会逐渐变大（持续追加）
            if (File.Exists(dst))
            {
                File.Delete(dst);
            }


            var bundle = helper.LoadBundleFile(src, true);

            using (var stream = File.OpenWrite(dst))
            {
                // 清除header里面的魔法数字
                var asset = helper.LoadAssetsFileFromBundle(bundle, 0);
                asset.file.header.unknown2 = 0;

                var replacer = new BundleReplacerFromAssets(asset.name, asset.name, asset.file, new List<AssetsReplacer>());

                // 将修改写入文件
                bundle.file.Write(new AssetsFileWriter(stream), new List<BundleReplacer>() { replacer });
            }

        }
    }
}
