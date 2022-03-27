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
using Lotus.Object3D;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup Object3DBase
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Селектор шаблона данных для отображения иерархии элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CEntity3DDataSelector : DataTemplateSelector
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
			/// Шаблон для представления меша
			/// </summary>
			public DataTemplate Mesh { get; set; }

			/// <summary>
			/// Шаблон для набора мешей
			/// </summary>
			public DataTemplate MeshSet { get; set; }

			/// <summary>
			/// Шаблон для представления материала
			/// </summary>
			public DataTemplate Material { get; set; }

			/// <summary>
			/// Шаблон для представления текстурного слота
			/// </summary>
			public DataTemplate TextureSlot { get; set; }

			/// <summary>
			/// Шаблон для представления набора материалов
			/// </summary>
			public DataTemplate MaterialSet { get; set; }
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
				CScene3D scene = item as CScene3D;
				if (scene != null)
				{
					return (Scene);
				}

				CNode3D node = item as CNode3D;
				if (node != null)
				{
					return (Node);
				}

				CModel3D model = item as CModel3D;
				if (model != null)
				{
					return (Model);
				}

				CMesh3Df mesh = item as CMesh3Df;
				if (mesh != null)
				{
					return (Mesh);
				}

				CMeshSet mesh_set = item as CMeshSet;
				if (mesh_set != null)
				{
					return (MeshSet);
				}

				CMaterial material = item as CMaterial;
				if (material != null)
				{
					return (Material);
				}

				CTextureSlot texture_slot = item as CTextureSlot;
				if (texture_slot != null)
				{
					return (TextureSlot);
				}

				CMaterialSet material_set = item as CMaterialSet;
				if (material_set != null)
				{
					return (MaterialSet);
				}

				return (Scene);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================