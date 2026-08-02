using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ScreenTranslate.Models;
namespace ScreenTranslate.Services
{
 public sealed class OcrService:IOcrService
 {
  public Task<OcrResult> ExtractTextAsync(Bitmap image,string sourceLanguageCode)
  { if(image==null)throw new ArgumentNullException("image");return Task.Run(delegate{return Extract(image,sourceLanguageCode);}); }
  static OcrResult Extract(Bitmap image,string code)
  {
   var language=Map(code);var exe=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ocr","tesseract.exe");if(!File.Exists(exe))throw new FileNotFoundException("Tesseract OCR غير موجود.",exe);
   var start=new ProcessStartInfo(exe,"stdin stdout -l "+language+" --psm 6"){UseShellExecute=false,CreateNoWindow=true,RedirectStandardInput=true,RedirectStandardOutput=true,RedirectStandardError=true,StandardOutputEncoding=Encoding.UTF8};
   using(var process=Process.Start(start))using(var memory=new MemoryStream())
   {
    image.Save(memory,ImageFormat.Png);memory.Position=0;memory.CopyTo(process.StandardInput.BaseStream);process.StandardInput.Close();var text=process.StandardOutput.ReadToEnd().Trim();var error=process.StandardError.ReadToEnd();if(!process.WaitForExit(20000)){try{process.Kill();}catch{}throw new TimeoutException("انتهت مهلة OCR.");}if(process.ExitCode!=0)throw new InvalidOperationException("تعذر تشغيل OCR: "+error);
    var detected=string.Equals(code,"auto",StringComparison.OrdinalIgnoreCase)?"auto":code;return new OcrResult{Text=text,DetectedLanguageCode=detected,DetectedLanguageNameArabic=Name(detected)};
   }
  }
  static string Map(string code){switch(code){case"fr":return"fra";case"de":return"deu";case"es":return"spa";case"ar":return"ara";case"en":return"eng";default:return"eng+fra+deu+spa+ara";}}
  static string Name(string code){switch(code){case"fr":return"الفرنسية";case"de":return"الألمانية";case"es":return"الإسبانية";case"ar":return"العربية";case"en":return"الإنجليزية";default:return"كشف تلقائي";}}
 }
}
