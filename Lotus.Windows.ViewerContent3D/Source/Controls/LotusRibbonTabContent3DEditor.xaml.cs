//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Модуль работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для ленты
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusRibbonTabContent3DEditor.xaml.cs
*		Контекстная вкладка ленты для просмотра свойств и редактирования 3D контента.
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
using System.Windows.Media.Media3D;
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
		/// Контекстная вкладка ленты для просмотра свойств и редактирования 3D контента
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusRibbonTabContent3DEditor : RibbonTabItem
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Основной редактор 3D контента
			/// </summary>
			public static readonly DependencyProperty Content3DViewEditorProperty = DependencyProperty.Register(nameof(Content3DViewEditor),
				typeof(LotusViewerContent3D),
				typeof(LotusRibbonTabContent3DEditor),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Основной редактор 3D контента
			/// </summary>
			public LotusViewerContent3D Content3DViewEditor
			{
				get { return (LotusViewerContent3D)GetValue(Content3DViewEditorProperty); }
				set { SetValue(Content3DViewEditorProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusRibbonTabContent3DEditor()
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
			private void OnRibbonTabContent3DEditor_Loaded(Object sender, RoutedEventArgs args)
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
				if (Content3DViewEditor != null)
				{
					Content3DViewEditor.OpenFile(null, null);
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
				if (Content3DViewEditor != null)
				{
					Content3DViewEditor.SaveFile();
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
				if (Content3DViewEditor != null)
				{
					Content3DViewEditor.SaveAsFile(null, null);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка вверх оси Y
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioSetYUp_Checked(Object sender, RoutedEventArgs args)
			{
				if (Content3DViewEditor != null)
				{
					Content3DViewEditor.Helix3DViewer.ModelUpDirection = new Vector3D(0, 1, 0);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка вверх оси Z
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioSetZUp_Checked(Object sender, RoutedEventArgs args)
			{
				if (Content3DViewEditor != null)
				{
					Content3DViewEditor.Helix3DViewer.ModelUpDirection = new Vector3D(0, 0, 1);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инверсия по оси X
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnCheckBoxInverseX_Checked(Object sender, RoutedEventArgs args)
			{
				if (Content3DViewEditor != null)
				{
					//Content3DViewEditor.Helix3DViewer.Inver = new Vector3D(0, 0, 1);
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