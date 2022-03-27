//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Модуль работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для ленты
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusRibbonTabTextEditor.xaml.cs
*		Контекстная вкладка ленты для просмотра свойств и редактирования текстового содержания.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
//---------------------------------------------------------------------------------------------------------------------
using Fluent;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsWPFControlsRibbon
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Контекстная вкладка ленты для просмотра свойств и редактирования текстового содержания
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusRibbonTabTextEditor : RibbonTabItem
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Основной текстовый редактор
			/// </summary>
			public static readonly DependencyProperty TextViewEditorProperty = DependencyProperty.Register(nameof(TextViewEditor),
				typeof(LotusViewerText),
				typeof(LotusRibbonTabTextEditor),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Основной текстовый редактор
			/// </summary>
			public LotusViewerText TextViewEditor
			{
				get { return (LotusViewerText)GetValue(TextViewEditorProperty); }
				set { SetValue(TextViewEditorProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusRibbonTabTextEditor()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(RibbonTabItem));
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка вкладки ленты
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRibbonTabTextEditor_Loaded(Object sender, RoutedEventArgs args)
			{
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
				Encoding[] encodings = new Encoding[]
				{
					Encoding.ASCII,
					Encoding.UTF8,
					Encoding.GetEncoding(1251),
					Encoding.Unicode,
					Encoding.UTF32,
					Encoding.Latin1,
					Encoding.BigEndianUnicode
				};
				comboBoxEncodings.ItemsSource = encodings;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие файла
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonOpen_Click(Object sender, RoutedEventArgs args)
			{
				if (TextViewEditor != null)
				{
					TextViewEditor.OpenFile(null, null);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие файла в программе Notepad
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonOpenNotepad_Click(Object sender, RoutedEventArgs args)
			{
				if (TextViewEditor != null && XFilePath.CheckCorrectFileName(TextViewEditor.FileName))
				{
					String file_name_param = XChar.DoubleQuotes + TextViewEditor.FileName + XChar.DoubleQuotes;
					XNative.ShellExecute(IntPtr.Zero, "open", "notepad++", file_name_param, null, TShowCommands.SW_NORMAL);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранение документа
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonSave_Click(Object sender, RoutedEventArgs args)
			{
				if (TextViewEditor != null)
				{
					TextViewEditor.SaveFile();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранение документа под новым именем
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonSaveAs_Click(Object sender, RoutedEventArgs args)
			{
				if (TextViewEditor != null)
				{
					TextViewEditor.SaveAsFile(null, null);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор кодировки для отображения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnComboBoxEncodings_SelectionChanged(Object sender, SelectionChangedEventArgs args)
			{
				if (TextViewEditor != null)
				{
					TextViewEditor.ChangedEncoding(comboBoxEncodings.SelectedItem as Encoding);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование выделенного текста в буфер обмен
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonCopy_Click(Object sender, RoutedEventArgs args)
			{
				if (TextViewEditor != null)
				{
					TextViewEditor.AvalonTextEditor.Copy();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выставка из буфера обмена текста
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonPaste_Click(Object sender, RoutedEventArgs args)
			{
				if (TextViewEditor != null)
				{
					TextViewEditor.AvalonTextEditor.Paste();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор формата подсветки синтаксиса
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnComboBoxSyntaxHighlighting_SelectionChanged(Object sender, SelectionChangedEventArgs args)
			{
				if (TextViewEditor == null) return;
				TextViewEditor.ChangedSyntaxHighlighting(comboBoxSyntaxHighlighting.SelectedItem as String);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================