//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Модуль работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для ленты
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusRibbonTabImageEditor.xaml.cs
*		Контекстная вкладка ленты для просмотра свойств и редактирования изображения.
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
		/// Контекстная вкладка ленты для просмотра свойств и редактирования изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusRibbonTabImageEditor : RibbonTabItem
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Основной редактор изображения
			/// </summary>
			public static readonly DependencyProperty ImageViewEditorProperty = DependencyProperty.Register(nameof(ImageViewEditor),
				typeof(LotusViewerImage),
				typeof(LotusRibbonTabImageEditor),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Основной редактор изображения
			/// </summary>
			public LotusViewerImage ImageViewEditor
			{
				get { return (LotusViewerImage)GetValue(ImageViewEditorProperty); }
				set { SetValue(ImageViewEditorProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusRibbonTabImageEditor()
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
			private void OnRibbonTabImageEditor_Loaded(Object sender, RoutedEventArgs args)
			{
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
				if (ImageViewEditor != null)
				{
					ImageViewEditor.OpenFile(null, null);
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

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранение файла
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonSave_Click(Object sender, RoutedEventArgs args)
			{
				if (ImageViewEditor != null)
				{
					ImageViewEditor.SaveFile();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранение файла под новым именем
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonSaveAs_Click(Object sender, RoutedEventArgs args)
			{
				if (ImageViewEditor != null)
				{
					ImageViewEditor.SaveAsFile(null, null);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор маски отображения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioChannelImage_Checked(Object sender, RoutedEventArgs args)
			{
				if (ImageViewEditor != null)
				{
					if (radioChannelOriginal.IsChecked.Value)
					{
						ImageViewEditor.SetViewOriginal();
					}

					if (radioChannelAlpha.IsChecked.Value)
					{
						ImageViewEditor.SetViewAlpha();
					}

					if (radioChannelNoTransparent.IsChecked.Value)
					{
						ImageViewEditor.SetViewNoTransparent();
					}
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