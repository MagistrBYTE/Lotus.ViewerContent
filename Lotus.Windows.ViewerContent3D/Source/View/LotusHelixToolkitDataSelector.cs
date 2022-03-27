//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Общий модуль Windows
// Подраздел: Подсистема 3D контента
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusObject3DDataSelector.cs
*		Селекторы для выбора модели отображения данных.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
//---------------------------------------------------------------------------------------------------------------------
using Helix = HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.SharpDX.Core.Assimp;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Object3D;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsCommonContent3D
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Селектор шаблона данных для отображения иерархии элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CHelixToolkitDataSelector : DataTemplateSelector
		{
			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Шаблон для представления сцены
			/// </summary>
			public DataTemplate Scene { get; set; }

			/// <summary>
			/// Шаблон для представления узла
			/// </summary>
			public DataTemplate Node { get; set; }

			/// <summary>
			/// Шаблон для представления модели
			/// </summary>
			public DataTemplate Model { get; set; }

			/// <summary>
			/// Шаблон для представления неизвестного узла
			/// </summary>
			public DataTemplate Unknow { get; set; }
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор шаблона привязки данных
			/// </summary>
			/// <param name="item">Объект</param>
			/// <param name="container">Контейнер</param>
			/// <returns>Нужный шаблон</returns>
			//---------------------------------------------------------------------------------------------------------
			public override DataTemplate SelectTemplate(Object item, DependencyObject container)
			{
				GroupNode node = item as GroupNode;
				if (node != null)
				{
					return (Node);
				}

				MeshNode model = item as MeshNode;
				if (model != null)
				{
					return (Model);
				}

				SceneNode scene = item as SceneNode;
				if (scene != null)
				{
					return (Scene);
				}

				return (Unknow);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================