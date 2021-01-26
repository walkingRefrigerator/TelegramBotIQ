using System;
using System.IO;
using System.Threading.Tasks;

using SautinSoft;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace ConvertPDF
{
    public class Convert
    {

        public static async Task ConvertPDFtoWord(TelegramBotClient bot, Message msg)
        {
            if (msg.Document.FileName.Contains(".pdf"))
            {

                string path = String.Format(@"TelegramBotIQ Users File\Convert\{0}.pdf", msg.Document.FileName);
                string file_id = msg.Document.FileId;

                try
                {
                    using (var filestream = System.IO.File.OpenWrite(path))
                    {
                        var filedowload = await bot.GetInfoAndDownloadFileAsync(
                            fileId: file_id,
                            destination: filestream
                            );
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    await bot.SendTextMessageAsync(msg.Chat.Id, "Возникла ошибка :(" + Environment.NewLine +
                        "Не беспокойтесь, разрабу дадим по шапке");
                }


                PdfFocus f = new PdfFocus();

                string pathDocx = String.Format(@"TelegramBotIQ Users File\Convert\{0}.docx", msg.Document.FileName);

                f.OpenPdf(path);

                if (f.PageCount > 0)
                {
                    await bot.SendTextMessageAsync(msg.Chat.Id, "Подождите, идет конвертирование!");

                    f.WordOptions.Format = PdfFocus.CWordOptions.eWordDocument.Docx;
                    f.ToWord(pathDocx);
                }

                FileStream fileStream = System.IO.File.OpenRead(pathDocx);

                InputOnlineFile file = new InputOnlineFile(fileStream);

                await bot.SendDocumentAsync(msg.Chat.Id, file, caption: "Конвертирование завершено!");
            }
            else
            {
                await bot.SendTextMessageAsync(msg.Chat.Id, "Файл имеет не верный формат");
                return;
            }
        }

    }
}
