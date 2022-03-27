//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Модуль работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для просмотра и редактирования файлов
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusViewerText.xaml.cs
*		Элемент для просмотра и редактирования файлов в текстовом формате.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//---------------------------------------------------------------------------------------------------------------------
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Utils;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsWPFControlsViewerFiles
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Allows producing foldings from a document based on braces
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class BraceFoldingStrategy
		{
			/// <summary>
			/// Gets/Sets the opening brace. The default value is '{'.
			/// </summary>
			public Char OpeningBrace { get; set; }

			/// <summary>
			/// Gets/Sets the closing brace. The default value is '}'.
			/// </summary>
			public Char ClosingBrace { get; set; }

			/// <summary>
			/// Creates a new BraceFoldingStrategy.
			/// </summary>
			public BraceFoldingStrategy()
			{
				this.OpeningBrace = '{';
				this.ClosingBrace = '}';
			}

			public void UpdateFoldings(FoldingManager manager, TextDocument document)
			{
				Int32 firstErrorOffset;
				IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
				manager.UpdateFoldings(newFoldings, firstErrorOffset);
			}

			/// <summary>
			/// Create <see cref="NewFolding"/>s for the specified document.
			/// </summary>
			public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out Int32 firstErrorOffset)
			{
				firstErrorOffset = -1;
				return CreateNewFoldings(document);
			}

			/// <summary>
			/// Create <see cref="NewFolding"/>s for the specified document.
			/// </summary>
			public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
			{
				List<NewFolding> newFoldings = new List<NewFolding>();

				Stack<Int32> startOffsets = new Stack<Int32>();
				Int32 lastNewLineOffset = 0;
				Char openingBrace = this.OpeningBrace;
				Char closingBrace = this.ClosingBrace;
				for (Int32 i = 0; i < document.TextLength; i++)
				{
					Char c = document.GetCharAt(i);
					if (c == openingBrace)
					{
						startOffsets.Push(i);
					}
					else if (c == closingBrace && startOffsets.Count > 0)
					{
						Int32 startOffset = startOffsets.Pop();
						// don't fold if opening and closing brace are on the same line
						if (startOffset < lastNewLineOffset)
						{
							newFoldings.Add(new NewFolding(startOffset, i + 1));
						}
					}
					else if (c == '\n' || c == '\r')
					{
						lastNewLineOffset = i + 1;
					}
				}
				newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
				return newFoldings;
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент для просмотра и редактирования файлов в текстовом формате
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusViewerText : UserControl, ILotusViewerContentFile, INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			/// <summary>
			/// Список поддерживаемых форматов файлов
			/// </summary>
			public static readonly String[] SupportFormatFile = new String[]
			{
				".txt",
				".md",
				".cs",
				".xml",
				".php",
				".java",
				".info",
				".json",
				".js",
				".css"
			};
			#endregion

			#region ======================================= СТАТИЧЕСКИЕ МЕТОДЫ ========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на поддерживаемый формат файла
			/// </summary>
			/// <param name="extension">Расширение файла</param>
			/// <returns>Статус поддержки</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Boolean IsSupportFormatFile(String extension)
			{
				return (SupportFormatFile.Contains(extension));
			}
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Имя файла
			/// </summary>
			public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName),
				typeof(String),
				typeof(LotusViewerText),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal FoldingManager mFoldingManager;
			protected internal System.Object mFoldingStrategy;

			// Reasonable max and min font size values
			private const Double FONT_MAX_SIZE = 60d;
			private const Double FONT_MIN_SIZE = 5d;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Имя файла
			/// </summary>
			public String FileName
			{
				get { return (String)GetValue(FileNameProperty); }
				set { SetValue(FileNameProperty, value); }
			}

			/// <summary>
			/// Основной текстовый редактор
			/// </summary>
			public ICSharpCode.AvalonEdit.TextEditor AvalonTextEditor
			{
				get { return (avalonTextEditor); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusViewerText()
			{
				InitializeComponent();
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusViewerContentFile ============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание нового файла с указанным именем и параметрами
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			/// <param name="parameters_create">Параметры создания файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void NewFile(String file_name, CParameters parameters_create)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие указанного файла
			/// </summary>
			/// <param name="file_name">Полное имя файла</param>
			/// <param name="parameters_open">Параметры открытия файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void OpenFile(String file_name, CParameters parameters_open)
			{
				// Если файл пустой то используем диалог
				if(String.IsNullOrEmpty(file_name))
				{
					file_name = XFileDialog.Open("Открыть документ", null);
					if (file_name.IsExists())
					{
						// Загружаем файл
						AvalonTextEditor.Load(file_name);

						FileName = file_name;
						XLogger.LogInfoModule(nameof(LotusViewerText), $"Открыт файл с именем: [{FileName}]");
					}
				}
				else
				{
					// Загружаем файл
					AvalonTextEditor.Load(file_name);
					FileName = file_name;
					XLogger.LogInfoModule(nameof(LotusViewerText), $"Открыт файл с именем: [{FileName}]");
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранения файла
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SaveFile()
			{
				// Если имя файла есть
				if (String.IsNullOrEmpty(FileName) == false)
				{
					AvalonTextEditor.Save(FileName);
					XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
				}
				else
				{
					String file_name = XFileDialog.Save("Сохранить документ", null);
					if (XFilePath.CheckCorrectFileName(file_name))
					{
						AvalonTextEditor.Save(file_name);
						FileName = file_name;
						XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранение файла под новым именем и параметрами
			/// </summary>
			/// <param name="file_name">Полное имя файла</param>
			/// <param name="parameters_save">Параметры сохранения файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void SaveAsFile(String file_name, CParameters parameters_save)
			{
				if (String.IsNullOrEmpty(file_name))
				{
					if (String.IsNullOrEmpty(FileName) == false)
					{
						String dir = Path.GetDirectoryName(FileName);
						String file = Path.GetFileNameWithoutExtension(FileName);
						String ext = Path.GetExtension(FileName).Remove(0, 1);

						 file_name = XFileDialog.Save("Сохранить документ как", dir, file, ext);
						if (XFilePath.CheckCorrectFileName(file_name))
						{
							AvalonTextEditor.Save(file_name);
							FileName = file_name;
							XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
						}
					}
					else
					{
						file_name = XFileDialog.Save("Сохранить документ как", null);
						if (XFilePath.CheckCorrectFileName(file_name))
						{
							AvalonTextEditor.Save(file_name);
							FileName = file_name;
							XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
						}
					}
				}
				else
				{
					if (XFilePath.CheckCorrectFileName(file_name))
					{
						AvalonTextEditor.Save(file_name);
						FileName = file_name;
						XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
					}
				}
			}

			//-------------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Печать файла
			/// </summary>
			/// <param name="parameters_print">Параметры печати файла</param>
			//-------------------------------------------------------------------------------------------------------------
			public void PrintFile(CParameters parameters_print)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Экспорт файла под указанным именем и параметрами
			/// </summary>
			/// <param name="file_name">Полное имя файла</param>
			/// <param name="parameters_export">Параметры для экспорта файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void ExportFile(String file_name, CParameters parameters_export)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие файла
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void CloseFile()
			{
				AvalonTextEditor.Clear();
				FileName = "";
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление статус сворачивания
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateFoldings()
			{
				if (mFoldingStrategy is BraceFoldingStrategy)
				{
					((BraceFoldingStrategy)mFoldingStrategy).UpdateFoldings(mFoldingManager, AvalonTextEditor.Document);
				}
				if (mFoldingStrategy is XmlFoldingStrategy)
				{
					((XmlFoldingStrategy)mFoldingStrategy).UpdateFoldings(mFoldingManager, AvalonTextEditor.Document);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смена подсветки синтаксиса
			/// </summary>
			/// <param name="syntax">Язык подсветки синтаксиса</param>
			//---------------------------------------------------------------------------------------------------------
			public void ChangedSyntaxHighlighting(String syntax)
			{
				if (AvalonTextEditor.SyntaxHighlighting == null)
				{
					mFoldingStrategy = null;
				}
				else
				{
					switch (AvalonTextEditor.SyntaxHighlighting.Name)
					{
						case "XML":
							mFoldingStrategy = new XmlFoldingStrategy();
							AvalonTextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
							break;
						case "C#":
						case "C++":
						case "PHP":
						case "Java":
							AvalonTextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(AvalonTextEditor.Options);
							mFoldingStrategy = new BraceFoldingStrategy();
							break;
						default:
							AvalonTextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
							mFoldingStrategy = null;
							break;
					}
				}
				if (mFoldingStrategy != null)
				{
					if (mFoldingManager == null)
						mFoldingManager = FoldingManager.Install(AvalonTextEditor.TextArea);
					UpdateFoldings();
				}
				else
				{
					if (mFoldingManager != null)
					{
						FoldingManager.Uninstall(mFoldingManager);
						mFoldingManager = null;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смена кодировки
			/// </summary>
			/// <param name="encoding">Кодировка</param>
			//---------------------------------------------------------------------------------------------------------
			public void ChangedEncoding(Encoding encoding)
			{
				AvalonTextEditor.Text = FileReader.ReadFileContent(FileName, encoding);
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка пользовательского элемента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnUserControl_Loaded(Object sender, RoutedEventArgs args)
			{
				SearchPanel.Install(AvalonTextEditor);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутка колеса мыши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnAvalonTextEditor_PreviewMouseWheel(Object sender, MouseWheelEventArgs args)
			{
				Boolean ctrl = Keyboard.Modifiers == ModifierKeys.Control;
				if (ctrl)
				{
					Boolean increase = args.Delta > 0;
					Double currentSize = avalonTextEditor.FontSize;

					if (increase)
					{
						if (currentSize < FONT_MAX_SIZE)
						{
							Double newSize = Math.Min(FONT_MAX_SIZE, currentSize + 1);
							avalonTextEditor.FontSize = newSize;
						}
					}
					else
					{
						if (currentSize > FONT_MIN_SIZE)
						{
							Double newSize = Math.Max(FONT_MIN_SIZE, currentSize - 1);
							avalonTextEditor.FontSize = newSize;
						}
					}

					args.Handled = true;
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства.
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства.
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================