using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;

namespace kp1
{
    class Program
    {
        public static string Tree(string dir)
        {
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string d in dirs)
            {
                if (dirs == null)
                {
                    Console.WriteLine("Директория пуста");
                }
                else
                {
                    try
                    {
                        Console.WriteLine(d + "->");
                        string[] _dirs = Directory.GetDirectories(d);
                        foreach (string D in _dirs)
                        {
                            Console.WriteLine("   " + D);
                        }
                        string[] files = Directory.GetFiles(d);
                        foreach (string f in files)
                        {
                            Console.WriteLine("   " + f);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.Write("Невозожно посмотреть содержимое" + "\n");
                    }
                }
            }
            return dir;
        }
        public static double sizeFolder(string folder, ref double catalogSize)
        {
            try
            {
                //В переменную catalogSize будем записывать размеры всех файлов, с каждым
                //новым файлом перезаписывая данную переменную
                DirectoryInfo di = new DirectoryInfo(folder);
                DirectoryInfo[] diA = di.GetDirectories();
                FileInfo[] fi = di.GetFiles();
                //В цикле пробегаемся по всем файлам директории di и складываем их размеры
                foreach (FileInfo f in fi)
                {
                    catalogSize = catalogSize + f.Length;
                }
                //В цикле пробегаемся по всем вложенным директориям директории di
                foreach (DirectoryInfo df in diA)
                {
                    //рекурсивно вызываем метод
                    sizeFolder(df.FullName, ref catalogSize);
                }
                //1ГБ = 1024 Байта * 1024 КБайта * 1024 МБайта
                return Math.Round((double)(catalogSize/1024/1024/1024), 1);
            }
            //Начинаем перехватывать ошибки
            //DirectoryNotFoundException - директория не найдена
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Директория не найдена");
                return 0;
            }
            //UnauthorizedAccessException - отсутствует доступ к файлу или папке
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Отсутствует доступ");
                return 0;
            }
            catch (Exception)
            {
                Console.WriteLine("Произошла ошибка, обратитесь к администратору vip@bk.ru");
                return 0;
            }
        }
        public static void CopyDir(DirectoryInfo source, DirectoryInfo destination)
        {
            if(!destination.Exists)
            {
                destination.Create();
            }
            //копирование всех файлов
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name));
            }
            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory.
                string destianantionDir = Path.Combine(destination.FullName, dir.Name);
                // Call CopyDirectory() recursively.
                CopyDir(dir, new DirectoryInfo(destianantionDir));
            }
        }

        public static string Command(string[] arrparams, string _dir, string c, string copyDir)
        {

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            for (int i = 0; i < arrparams.Length; i++)
            {
                c = arrparams[0];
                _dir = arrparams[1];
                copyDir = arrparams[i];
            }
            FileInfo fileInfo = new FileInfo(_dir);
            DirectoryInfo dirInfo = new DirectoryInfo(_dir);
            DirectoryInfo copyDirInfo = new DirectoryInfo(copyDir);
            if (c == "open")
            {
                try
                {
                    if (dirInfo.Exists)
                    {
                        Tree(_dir);
                        config.AppSettings.Settings["startDir"].Value = _dir;
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                    else if (fileInfo.Exists)
                    {
                        using (FileStream fs = File.OpenRead($"{fileInfo}"))
                        {
                            byte[] arr = new byte[fs.Length];
                            fs.Read(arr, 0, arr.Length);
                            string textFile = System.Text.Encoding.Default.GetString(arr);
                            Console.WriteLine($"{textFile}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет такого элемента");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else if (c == "info")
            {
                try
                {
                    if (dirInfo.Exists)
                    {
                        double catalogSize = 0;
                        catalogSize = sizeFolder(_dir, ref catalogSize);
                        if (catalogSize != 0)
                        {
                            Console.WriteLine($"Название каталога: {dirInfo.Name}\n" +
                                $"Полное название каталога: {dirInfo.FullName}\n" +
                                $"Время создания каталога: {dirInfo.CreationTime}\n" +
                                $"Корневой каталог: {dirInfo.Root}\n" +
                                $"Размер каталога: {catalogSize} Гб");
                        }
                        else
                        {
                            Console.WriteLine($"Название каталога: {dirInfo.Name}\n" +
                                $"Полное название каталога: {dirInfo.FullName}\n" +
                                $"Время создания каталога: {dirInfo.CreationTime}\n" +
                                $"Корневой каталог: {dirInfo.Root}\n" +
                                $"Размер каталога: пуст");
                        }
                    }
                    else if (fileInfo.Exists)
                    {
                        Console.WriteLine($"Название файла: {fileInfo.Name}\n" +
                            $"Полное название файла: {fileInfo.FullName}\n" +
                            $"Расширение файла: {fileInfo.Extension}\n" +
                            $"Размер файла: {fileInfo.Length} Кб\n" +
                            $"Путь к родительскому каталогу: {fileInfo.DirectoryName}\n" +
                            $"Время создания: {fileInfo.CreationTime}");
                    }
                    else
                    {
                        Console.WriteLine("Нет такого элемента");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else if (c == "create")
            {
                try
                {
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }
                    else if (!fileInfo.Exists)
                    {
                        fileInfo.Create();
                    }
                    else if (dirInfo.Exists)
                    {
                        Console.WriteLine("Такая директория уже есть");
                    }
                    else
                    {
                        Console.WriteLine("Такой файл уже есть");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Каталог уже существует");
                }
                catch (IOException)
                {
                    Console.WriteLine("Такой файл уже существует");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else if (c == "delete")
            {
                try
                {
                    if (dirInfo.Exists)
                    {
                        dirInfo.Delete(true);
                    }
                    else if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                    else
                    {
                        Console.WriteLine("Нет данного пути");
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Папка не пуста, очистите перед удалением");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else if (c == "copy")
            {
                try
                {
                    if (dirInfo.Exists)
                    {
                        CopyDir(dirInfo, copyDirInfo);
                    }
                    else if (fileInfo.Exists && dirInfo.Exists && copyDirInfo.Exists)
                    {
                        fileInfo.CopyTo(copyDir, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex);
                }
            }
            return Convert.ToString(arrparams);
        }
        static void Main(string[] args)
        {
            string c = "";
            string _dir = "";
            string copyDir = "";
            string dir = "C:\\";
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings["startDir"].Value == dir || config.AppSettings.Settings["startDir"].Value == "")
            {
                Tree(dir);
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Введите команду, для нажмите Ctr+C");
                        string command = Console.ReadLine();
                        string[] arrparams = Regex.Matches(command, @"\""(?<token>.+?)\""|(?<token>[^\s]+)")
                                .Cast<Match>()
                                .Select(m => m.Groups["token"].Value)
                                .ToArray();
                        if (arrparams != null)
                        {
                            Command(arrparams, _dir, c, copyDir);
                        }
                        else
                        {
                            Console.WriteLine("Ввели пустую строку");
                        }
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Ввели пустую строку");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                _dir = config.AppSettings.Settings["startDir"].Value;
                Tree(_dir);
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Введите команду, для выхода нажмите Ctr+C");
                        string command = Console.ReadLine();
                        string[] arrparams = Regex.Matches(command, @"\""(?<token>.+?)\""|(?<token>[^\s]+)")
                                .Cast<Match>()
                                .Select(m => m.Groups["token"].Value)
                                .ToArray();
                        if (arrparams != null)
                        {
                            Command(arrparams, _dir, c, copyDir);
                        }
                        else
                        {
                            Console.WriteLine("Ввели пустую строку");
                        }
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Ввели пустую строку");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    
                }
            }
        }
    }
}