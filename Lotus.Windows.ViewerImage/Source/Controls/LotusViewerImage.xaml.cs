//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Модуль работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для просмотра и редактирования файлов
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusViewerImage.xaml.cs
*		Элемент для просмотра и редактирования файлов в формате изображения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
//---------------------------------------------------------------------------------------------------------------------
using FreeImageAPI;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Windows;
using System.Drawing.Imaging;
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
		/// Элемент для просмотра и редактирования файлов в формате изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusViewerImage : UserControl, ILotusViewerContentFile, INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			/// <summary>
			/// Список поддерживаемых форматов файлов
			/// </summary>
			public static readonly String[] SupportFormatFile = new String[] 
			{
				".bmp",
				".jpeg",
				".jpg",
				".png",
				".tiff",
				".tif",
				".psd",
				".tga",
				".targa",
				".gif",
				".hdr",
				".dds"
			};

			//
			// Константы для информирования об изменении свойств
			//
			protected static PropertyChangedEventArgs PropertyArgsImageWidth = new PropertyChangedEventArgs(nameof(ImageWidth));
			protected static PropertyChangedEventArgs PropertyArgsImageHeight = new PropertyChangedEventArgs(nameof(ImageHeight));
			protected static PropertyChangedEventArgs PropertyArgsImageResolutionX = new PropertyChangedEventArgs(nameof(ImageResolutionX));
			protected static PropertyChangedEventArgs PropertyArgsImageResolutionY = new PropertyChangedEventArgs(nameof(ImageResolutionY));
			protected static PropertyChangedEventArgs PropertyArgsImageFormat = new PropertyChangedEventArgs(nameof(ImageFormat));
			protected static PropertyChangedEventArgs PropertyArgsImageImageType = new PropertyChangedEventArgs(nameof(ImageType));
			protected static PropertyChangedEventArgs PropertyArgsImageColorType = new PropertyChangedEventArgs(nameof(ImageColorType));
			protected static PropertyChangedEventArgs PropertyArgsImageColorDepth = new PropertyChangedEventArgs(nameof(ImageColorDepth));
			protected static PropertyChangedEventArgs PropertyArgsImagePixelFormat = new PropertyChangedEventArgs(nameof(ImagePixelFormat));
			protected static PropertyChangedEventArgs PropertyArgsIsTransparentImage = new PropertyChangedEventArgs(nameof(IsTransparentImage));
			protected static PropertyChangedEventArgs PropertyArgsImageRedMask = new PropertyChangedEventArgs(nameof(ImageRedMask));
			protected static PropertyChangedEventArgs PropertyArgsImageGreenMask = new PropertyChangedEventArgs(nameof(ImageGreenMask));
			protected static PropertyChangedEventArgs PropertyArgsImageBlueMask = new PropertyChangedEventArgs(nameof(ImageBlueMask));
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка изображения по полному пути
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			/// <returns>Объект BitmapSource</returns>
			//---------------------------------------------------------------------------------------------------------
			public static BitmapSource LoadFromFile(String file_name)
			{
				// Format is stored in 'format' on successfull load.
				FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;

				// Try loading the file
				FIBITMAP dib = FreeImage.LoadEx(file_name, ref format);

				try
				{
					// Error handling
					if (dib.IsNull)
					{
						return (null);
					}

					BitmapSource image = FreeImage.GetBitmap(dib).ToBitmapSource();
					FreeImage.UnloadEx(ref dib);

					return (image);
				}
				catch(Exception exc)
				{
					XLogger.LogExceptionModule(nameof(LotusViewerImage), exc);
					return (null);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка изображения по полному пути
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			/// <param name="width">Требуемая ширина изображения</param>
			/// <param name="height">Требуемая высота изображения</param>
			/// <returns>Объект BitmapSource</returns>
			//---------------------------------------------------------------------------------------------------------
			public static BitmapSource LoadFromFile(String file_name, Int32 width, Int32 height)
			{
				// Format is stored in 'format' on successfull load.
				FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;

				// Try loading the file
				FIBITMAP dib = FreeImage.LoadEx(file_name, ref format);

				try
				{
					// Error handling
					if (dib.IsNull)
					{
						return (null);
					}

					BitmapSource image = FreeImage.GetBitmap(dib).ToBitmapSource(width, height);
					FreeImage.UnloadEx(ref dib);

					return (image);
				}
				catch (Exception exc)
				{
					XLogger.LogExceptionModule(nameof(LotusViewerImage), exc);
					return (null);
				}
			}
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Имя файла
			/// </summary>
			public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName),
				typeof(String),
				typeof(LotusViewerImage),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal String mFileName;
			protected internal Int32 mImageWidth;
			protected internal Int32 mImageHeight;
			protected internal Int32 mImageResolutionX;
			protected internal Int32 mImageResolutionY;
			protected internal FREE_IMAGE_FORMAT mFreeImageFormat;
			protected internal FREE_IMAGE_TYPE mFreeImageType;
			protected internal FREE_IMAGE_COLOR_TYPE mFreeImageColorType;
			protected internal Int32 mColorDepth;
			protected internal PixelFormat mPixelFormat;
			protected internal Boolean mIsTransparentImage;
			protected internal UInt32 mImageRedMask;
			protected internal UInt32 mImageGreenMask;
			protected internal UInt32 mImageBlueMask;

			protected internal FIBITMAP mFreeImageBitmap;
			protected internal Image mImagePresented;
			protected internal BitmapSource mBitmapOriginal;
			protected internal BitmapSource mBitmapAlpha;
			protected internal BitmapSource mBitmapNoTransparent;
			protected internal String mCurrentMessage;
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

			//
			// ПАРАМЕТРЫ ИЗОБРАЖЕНИЯ
			//
			/// <summary>
			/// Ширина изображения
			/// </summary>
			public Int32 ImageWidth
			{
				get
				{
					return (mImageWidth);
				}
			}

			/// <summary>
			/// Высота изображения
			/// </summary>
			public Int32 ImageHeight
			{
				get
				{
					return (mImageHeight);
				}
			}

			/// <summary>
			/// Разрешение изображения по X
			/// </summary>
			public Int32 ImageResolutionX
			{
				get
				{
					return (mImageResolutionX);
				}
			}

			/// <summary>
			/// Разрешение изображения по Y
			/// </summary>
			public Int32 ImageResolutionY
			{
				get
				{
					return (mImageResolutionY);
				}
			}

			/// <summary>
			/// Формат изображения
			/// </summary>
			public FREE_IMAGE_FORMAT ImageFormat
			{
				get
				{
					return (mFreeImageFormat);
				}
			}

			/// <summary>
			/// Тип изображения
			/// </summary>
			public FREE_IMAGE_TYPE ImageType
			{
				get
				{
					return (mFreeImageType);
				}
			}

			/// <summary>
			/// Тип цвета изображения
			/// </summary>
			public FREE_IMAGE_COLOR_TYPE ImageColorType
			{
				get
				{
					return (mFreeImageColorType);
				}
			}

			/// <summary>
			/// Глубина цвета
			/// </summary>
			public Int32 ImageColorDepth
			{
				get { return (mColorDepth); }
			}

			/// <summary>
			/// Формат пикселя
			/// </summary>
			public PixelFormat ImagePixelFormat
			{
				get { return (mPixelFormat); }
			}

			/// <summary>
			/// Статус прозрачности изображения
			/// </summary>
			public Boolean IsTransparentImage
			{
				get { return (mIsTransparentImage); }
			}

			/// <summary>
			/// Маска прозрачности для красного цвета
			/// </summary>
			public UInt32 ImageRedMask
			{
				get { return (mImageRedMask); }
			}

			/// <summary>
			/// Маска прозрачности для зеленого цвета
			/// </summary>
			public UInt32 ImageGreenMask
			{
				get { return (mImageGreenMask); }
			}

			/// <summary>
			/// Маска прозрачности для синего цвета
			/// </summary>
			public UInt32 ImageBlueMask
			{
				get { return (mImageBlueMask); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusViewerImage()
			{
				InitializeComponent();
				FreeImageEngine.Message += new OutputMessageFunction(OnFreeImageMessage);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Деструктор
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			~LotusViewerImage()
			{
				FreeImageEngine.Message -= new OutputMessageFunction(OnFreeImageMessage);
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
				if (String.IsNullOrEmpty(file_name))
				{
					file_name = XFileDialog.Open("Открыть изображение", "");
					if (file_name.IsExists())
					{
						// Загружаем файл
						Load(file_name);

						FileName = file_name;
						XLogger.LogInfoModule(nameof(LotusViewerImage), $"Открыт файл с именем: [{FileName}]");
					}
				}
				else
				{
					// Загружаем файл
					Load(file_name);
					FileName = file_name;
					XLogger.LogInfoModule(nameof(LotusViewerImage), $"Открыт файл с именем: [{FileName}]");
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранения файла
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SaveFile()
			{

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

					}
					else
					{

					}
				}
				else
				{
					if (XFilePath.CheckCorrectFileName(file_name))
					{

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
				imagePresent.Source = null;
				mBitmapOriginal = null;
				mBitmapAlpha = null;
				mBitmapNoTransparent = null;
				FileName = "";
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка изображения по полному пути
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void Load(String file_name)
			{
				if (!mFreeImageBitmap.IsNull)
				{
					mFreeImageBitmap.SetNull();
				}

				// Try loading the file
				mFreeImageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
				mFreeImageBitmap = FreeImage.LoadEx(file_name, ref mFreeImageFormat);

				try
				{
					// Error handling
					if (mFreeImageBitmap.IsNull)
					{
						// Chech whether FreeImage generated an error messe
						if (mCurrentMessage != null)
						{
							XLogger.LogErrorFormatModule(nameof(LotusViewerImage), "File could not be loaded!\nError:{0}", mCurrentMessage);
						}
						else
						{
							XLogger.LogErrorModule(nameof(LotusViewerImage), "File could not be loaded!");
						}
						return;
					}


					mFileName = file_name;

					//
					// РАЗМЕР ИЗОБРАЖЕНИЯ
					//
					mImageWidth = (Int32)FreeImage.GetWidth(mFreeImageBitmap);
					mImageHeight = (Int32)FreeImage.GetHeight(mFreeImageBitmap);
					mImageResolutionX = (Int32)FreeImage.GetResolutionX(mFreeImageBitmap);
					mImageResolutionY = (Int32)FreeImage.GetResolutionY(mFreeImageBitmap);

					//
					// ПАРАМЕТРЫ ИЗОБРАЖЕНИЯ
					//
					mFreeImageType = FreeImage.GetImageType(mFreeImageBitmap);
					mFreeImageColorType = FreeImage.GetColorType(mFreeImageBitmap);

					//
					// ПАРАМЕТРЫ ЦВЕТА
					//
					mColorDepth = (Int32)FreeImage.GetBPP(mFreeImageBitmap);
					mPixelFormat = FreeImage.GetPixelFormat(mFreeImageBitmap);
					mIsTransparentImage = FreeImage.IsTransparent(mFreeImageBitmap);

					//
					// ПАРАМЕТРЫ МАСКИ
					//
					mImageRedMask = FreeImage.GetRedMask(mFreeImageBitmap);
					mImageGreenMask = FreeImage.GetGreenMask(mFreeImageBitmap);
					mImageBlueMask = FreeImage.GetBlueMask(mFreeImageBitmap);

					// Получаем презентатор
					if (mImagePresented == null) mImagePresented = contentViewer.Content as Image;

					// Основной режим
					mBitmapOriginal = FreeImage.GetBitmap(mFreeImageBitmap).ToBitmapSource();

					// Если есть прозрачность
					if (FreeImage.IsTransparent(mFreeImageBitmap) && FreeImage.GetBPP(mFreeImageBitmap) > 24)
					{
						// Получаем альфа-канал
						FIBITMAP bitmap_alpha = FreeImage.GetChannel(mFreeImageBitmap, FREE_IMAGE_COLOR_CHANNEL.FICC_ALPHA);
						if (!bitmap_alpha.IsNull)
						{
							mBitmapAlpha = FreeImage.GetBitmap(bitmap_alpha).ToBitmapSource();
							FreeImage.UnloadEx(ref bitmap_alpha);
						}

						// Преобразуем
						FIBITMAP bitmap_no_transparent = FreeImage.ConvertTo24Bits(mFreeImageBitmap);
						if (!bitmap_no_transparent.IsNull)
						{
							mBitmapNoTransparent = FreeImage.GetBitmap(bitmap_no_transparent).ToBitmapSource();
							FreeImage.UnloadEx(ref bitmap_no_transparent);
						}
					}

					mImagePresented.Source = mBitmapOriginal;
					mImagePresented.Width = mImageWidth;
					mImagePresented.Height = mImageHeight;
				}
				catch(Exception exc)
				{
					XLogger.LogExceptionModule(nameof(LotusViewerImage), exc);
				}

				// Always unload bitmap
				FreeImage.UnloadEx(ref mFreeImageBitmap);

				NotifyPropertyChanged(PropertyArgsImageWidth);
				NotifyPropertyChanged(PropertyArgsImageHeight);
				NotifyPropertyChanged(PropertyArgsImageResolutionX);
				NotifyPropertyChanged(PropertyArgsImageResolutionY);
				NotifyPropertyChanged(PropertyArgsImageFormat);
				NotifyPropertyChanged(PropertyArgsImageImageType);
				NotifyPropertyChanged(PropertyArgsImageColorType);
				NotifyPropertyChanged(PropertyArgsImageColorDepth);
				NotifyPropertyChanged(PropertyArgsImagePixelFormat);
				NotifyPropertyChanged(PropertyArgsIsTransparentImage);
				NotifyPropertyChanged(PropertyArgsImageRedMask);
				NotifyPropertyChanged(PropertyArgsImageGreenMask);
				NotifyPropertyChanged(PropertyArgsImageBlueMask);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отобразить оригинальное изображение
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SetViewOriginal()
			{
				if (mBitmapOriginal != null)
				{
					mImagePresented.Source = mBitmapOriginal;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отобразить альфа-канал изображения
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SetViewAlpha()
			{
				if (mBitmapAlpha != null)
				{
					mImagePresented.Source = mBitmapAlpha;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отобразить изображение без учета альфа канала
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SetViewNoTransparent()
			{
				if (mBitmapNoTransparent != null)
				{
					mImagePresented.Source = mBitmapNoTransparent;
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка элемента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnUserControl_Loaded(Object sender, RoutedEventArgs args)
			{
				Double min_width = 1;

				if (mImagePresented.Width > contentViewer.ViewportWidth - 20)
				{
					min_width = (contentViewer.ViewportWidth - 20) / mImagePresented.Width;
				}

				Double min_height = 1;
				if (mImagePresented.Height > contentViewer.ViewportHeight - 20)
				{
					min_height = (contentViewer.ViewportHeight - 20) / mImagePresented.Height;
				}

				contentViewer.ContentScale = Math.Min(min_width, min_height);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработка сообщений библиотеки FreeImage
			/// </summary>
			/// <param name="format_image">Формат изображения</param>
			/// <param name="message">Строка сообщения</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnFreeImageMessage(FREE_IMAGE_FORMAT format_image, String message)
			{
				if (this.mCurrentMessage == null)
				{
					this.mCurrentMessage = message;
				}
				else
				{
					this.mCurrentMessage += "\n" + message;
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
			/// Вспомогательный метод для нотификации изменений свойства
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
			/// Вспомогательный метод для нотификации изменений свойства
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