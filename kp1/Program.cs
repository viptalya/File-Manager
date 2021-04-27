using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;

namespace kp1
{
    class Program
    {
        //метод для построения дерева
        public static string Tree(string dir)
        {
            //создается массив из данных директории
            string[] dirs = Directory.GetDirectories(dir);
            string[] Files = Directory.GetFiles(dir);
            foreach (string d in dirs)
            {
                try
                {
                    //вывод дерева
                    foreach (string F in Files)
                    {
                        Console.WriteLine(F);
                    }
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
                catch (Exception)
                {
                    Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
                }
            }

            return dir;
        }
        //метод для вычисления размеров директории или файла
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
                Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
                return 0;
            }
        }
        //метод для копирования директории или файла
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
            //поиск подкаталогов
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                //получить каталог назначения
                string destianantionDir = Path.Combine(destination.FullName, dir.Name);
                //выозов рекурсии
                CopyDir(dir, new DirectoryInfo(destianantionDir));
            }
        }
        //метод открытия файлов или директорий
        public static string Open(string _dir)
        {
            //открытие app.config
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //создание нового файла по старому
            FileInfo fileInfo = new FileInfo(_dir);
            DirectoryInfo dirInfo = new DirectoryInfo(_dir);
            try
            {
                //если директория существует
                if (dirInfo.Exists)
                {
                    //вывод содержимого директории
                    Tree(_dir);
                    //запись в app.config названия директории, чтобы она открывалась при новом открытии приложения
                    config.AppSettings.Settings["startDir"].Value = _dir;
                    //сохранение записи
                    config.Save(ConfigurationSaveMode.Modified);
                    //обновление app.config
                    ConfigurationManager.RefreshSection("appSettings");
                }
                //если существует файл
                else if (fileInfo.Exists)
                {
                    //читаем содержимое файла
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
            catch (IOException)
            {
                Console.WriteLine("Невозможно прочитать содержимое");
            }
            catch (Exception)
            {
                Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
            }
            return _dir;
        }

        //метод вывода информации
        public static string Info(string _dir)
        {
            //создание нового файла по старому
            FileInfo fileInfo = new FileInfo(_dir);
            DirectoryInfo dirInfo = new DirectoryInfo(_dir);
            try
            {
                double catalogSize = 0;
                //вызов метода вычисления размеров
                catalogSize = sizeFolder(_dir, ref catalogSize);
                //если существует директория 
                if (dirInfo.Exists)
                {
                    //если размер не 0
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
                //если существует файл
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
                    Console.WriteLine("Нет такого элемента или с ним невозможно взаимодействовать");
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
            }
            return _dir;
        }

        //метод создания директории или файла
        public static string Create(string _dir)
        {
            //создание нового файла по старому
            FileInfo fileInfo = new FileInfo(_dir);
            DirectoryInfo dirInfo = new DirectoryInfo(_dir);
            try
            {
                //поиск расширения файла
                string extension;
                extension = Path.GetExtension(_dir);
                //если не существует директории и расширение не задано
                if (!dirInfo.Exists && extension == "")
                {
                    //директория создается
                    dirInfo.Create();
                }
                //если не существует файла
                else if (!fileInfo.Exists)
                {
                    //создается файл
                    fileInfo.Create();
                }
                //если директория существует
                else if (dirInfo.Exists)
                {
                    Console.WriteLine("Такая директория уже есть");
                }
                //если файл существует
                else
                {
                    Console.WriteLine("Такой файл уже есть");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Каталог или файл уже существует");
            }
            catch (IOException)
            {
                Console.WriteLine("С этим нельзя взаимодействовать");
            }
            catch (Exception)
            {
                Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
            }
            return _dir;
        }

        //метод удаления
        public static string DeletE(string _dir)
        {
            //создание нового файла по старому
            FileInfo fileInfo = new FileInfo(_dir);
            DirectoryInfo dirInfo = new DirectoryInfo(_dir);
            try
            {
                //если директория существует
                if (dirInfo.Exists)
                {
                    //удаление директории
                    dirInfo.Delete(true);
                }
                //если файл существует
                else if (fileInfo.Exists)
                {
                    //удаление файла
                    fileInfo.Delete();
                }
                else
                {
                    Console.WriteLine("Нет данного пути");
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
            }
            return _dir;
        }

        //метод копирования
        public static string Copy(string _dir, string copyDir)
        {
            FileInfo fileInfo = new FileInfo(_dir);
            DirectoryInfo dirInfo = new DirectoryInfo(_dir);
            DirectoryInfo copyDirInfo = new DirectoryInfo(copyDir);
            try
            {
                //если директория существует 
                if (dirInfo.Exists && copyDirInfo.Exists)
                {
                    //копирование директории в директорию
                    CopyDir(dirInfo, copyDirInfo);
                }
                //если существует файл и директорию из которой копируем и в какую копируем
                else if (fileInfo.Exists && dirInfo.Exists && copyDirInfo.Exists)
                {
                    //копируем файл
                    fileInfo.CopyTo(copyDir, true);
                }
                else if (dirInfo.Exists && !copyDirInfo.Exists)
                {
                    copyDirInfo.Create();
                    CopyDir(dirInfo, copyDirInfo);
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
            }
            return _dir;
        }

        //метод команд
        public static string Command(string _dir, string c, string copyDir)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                Console.WriteLine("Введите команду, для нажмите Ctr+C");
                string command = Console.ReadLine();
                //разбиение введенной строки на слова
                string[] arrparams = Regex.Matches(command, @"\""(?<token>.+?)\""|(?<token>[^\s]+)")
                        .Cast<Match>()
                        .Select(m => m.Groups["token"].Value)
                        .ToArray();
                //если строка не пустая
                if (arrparams != null)
                {
                    //разбиение введенной строки на элементы
                    for (int i = 0; i < arrparams.Length; i++)
                    {
                        c = arrparams[0];
                        _dir = arrparams[1];
                        copyDir = arrparams[i];
                    }
                    //если пользователь ввел open
                    if (c == "open")
                    {
                        Open(_dir);
                    }
                    //если пользователь ввел info
                    else if (c == "info")
                    {
                        if (_dir.Substring(0, 2) != "C:")
                        {
                            string start = config.AppSettings.Settings["startDir"].Value + "\\" + _dir;
                            Info(start);
                        }
                        else
                        {
                            Info(_dir);
                        }
                    }
                    //если пользователь ввел create
                    else if (c == "create")
                    {
                        Create(_dir);
                    }
                    //если пользотель вводит delete
                    else if (c == "delete")
                    {
                        DeletE(_dir);
                    }
                    //если пользователь ввел copy
                    else if (c == "copy")
                    {
                        Copy(_dir, copyDir);
                    }
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
            catch (Exception)
            {
                Console.WriteLine($"У вас вышла ошибка, для исправления напишите на почту: vip_10@bk.ru");
            }
            return _dir;
        }
        static void Main(string[] args)
        {
            string c = "";
            string _dir = "";
            string copyDir = "";
            string dir = "C:\\";
            //работа с app.config
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                //если значение в app.config равно начальной директории или пустое, то открываем показываем папку С
                if (config.AppSettings.Settings["startDir"].Value == dir || config.AppSettings.Settings["startDir"].Value == "")
                {
                    Tree(dir);
                    while (true)
                    {
                        Command(_dir, c, copyDir);
                    }
                }
                else
                {
                    _dir = config.AppSettings.Settings["startDir"].Value;
                    Tree(_dir);
                    while (true)
                    {
                        Command(_dir, c, copyDir);
                    }
                }
            }
            catch
            {
                Tree(dir);
                while (true)
                {
                    Command(_dir, c, copyDir);
                }
            }
        }
    }
}