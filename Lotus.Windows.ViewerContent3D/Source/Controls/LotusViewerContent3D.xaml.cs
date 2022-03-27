//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Модуль работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для просмотра и редактирования файлов
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusViewerContent3D.xaml.cs
*		Элемент для просмотра и редактирования файлов формата 3D контента.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
//---------------------------------------------------------------------------------------------------------------------
using Helix = HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.Wpf.SharpDX;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Object3D;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsWPFControlsContent
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент для просмотра и редактирования файлов формата 3D контента
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusViewerContent3D : UserControl, ILotusViewerContentFile, IDisposable, INotifyPropertyChanged
		{
			#region ======================================= КОНСТАНТНЫЕ ДАННЫЕ ========================================
			/// <summary>
			/// Команда для увеличения выбранного объема. Аргумент - выбранный объём типа <see cref="Rect3D"/> 
			/// </summary>
			public const String COMMAND_ZOOM_EXTENTS = "ZoomExtents";

			/// <summary>
			/// Имя ортографической камеры
			/// </summary>
			public const String OrthographicCameraName = "Orthographic Camera";

			/// <summary>
			/// Имя перспективной камеры
			/// </summary>
			public const String PerspectiveCameraName = "Perspective Camera";
			#endregion

			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsShowWireframe = new PropertyChangedEventArgs(nameof(ShowWireframe));
			protected static PropertyChangedEventArgs PropertyArgsRenderFlat = new PropertyChangedEventArgs(nameof(RenderFlat));
			protected static PropertyChangedEventArgs PropertyArgsRenderEnvironmentMap = new PropertyChangedEventArgs(nameof(PropertyArgsRenderEnvironmentMap));

			protected static PropertyChangedEventArgs PropertyArgsCamera = new PropertyChangedEventArgs(nameof(Camera));
			protected static PropertyChangedEventArgs PropertyArgsCameraModel = new PropertyChangedEventArgs(nameof(CameraModel));
			
			protected static PropertyChangedEventArgs PropertyArgsEffectsManager = new PropertyChangedEventArgs(nameof(EffectsManager));

			protected static PropertyChangedEventArgs PropertyArgsScene = new PropertyChangedEventArgs(nameof(Scene));
			protected static PropertyChangedEventArgs PropertyArgsSceneRoot = new PropertyChangedEventArgs(nameof(SceneRoot));
			protected static PropertyChangedEventArgs PropertyArgsGroupModel = new PropertyChangedEventArgs(nameof(GroupModel));
			protected static PropertyChangedEventArgs PropertyArgsSelectedModel = new PropertyChangedEventArgs(nameof(SelectedModel));

			protected static PropertyChangedEventArgs PropertyArgsEnableAnimation = new PropertyChangedEventArgs(nameof(EnableAnimation));
			protected static PropertyChangedEventArgs PropertyArgsSelectedAnimation = new PropertyChangedEventArgs(nameof(SelectedAnimation));
			protected static PropertyChangedEventArgs PropertyArgsSpeedAnimation = new PropertyChangedEventArgs(nameof(SpeedAnimation));

			protected static PropertyChangedEventArgs PropertyArgsGridGeometry = new PropertyChangedEventArgs(nameof(GridGeometry));
			protected static PropertyChangedEventArgs PropertyArgsGridColor = new PropertyChangedEventArgs(nameof(GridColor));
			protected static PropertyChangedEventArgs PropertyArgsGridTransform = new PropertyChangedEventArgs(nameof(GridTransform));

			private static String OpenFileFilter = $"{HelixToolkit.SharpDX.Core.Assimp.Importer.SupportedFormatsString}";
			private static String ExportFileFilter = $"{HelixToolkit.SharpDX.Core.Assimp.Exporter.SupportedFormatsString}";
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
				return (OpenFileFilter.Contains(extension));
			}
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Имя файла
			/// </summary>
			public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName),
				typeof(String),
				typeof(LotusViewerContent3D),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Параметры визуализации
			internal Boolean mShowWireframe = false;
			internal Boolean mRenderFlat = false;
			internal Boolean mRenderEnvironmentMap = true;
			internal TextureModel mEnvironmentMap;

			// Камера
			internal String mCameraModel;
			internal Helix.Camera mCamera;
			internal Helix.OrthographicCamera mDefaultOrthographicCamera;
			internal Helix.PerspectiveCamera mDefaultPerspectiveCamera;

			// Рендер техника
			internal IEffectsManager mEffectsManager;

			// Базовые модели
			internal CScene3D mScene;
			internal SceneNode mSceneRoot;
			internal Helix.SceneNodeGroupModel3D mGroupModel;
			internal Helix.GeometryModel3D mSelectedModel;

			// Анимация
			internal Boolean mEnableAnimation = false;
			internal IList<Animation> mSceneAnimations;
			internal ObservableCollection<IAnimationUpdater> mAnimations;
			internal IAnimationUpdater mSelectedAnimation = null;
			internal IAnimationUpdater mAnimationUpdater;
			internal Single mSpeedAnimation = 1.0f;

			// Параметры скелета
			internal List<BoneSkinMeshNode> mBoneSkinNodes = new List<BoneSkinMeshNode>();
			internal List<BoneSkinMeshNode> mSkeletonNodes = new List<BoneSkinMeshNode>();
			internal CompositionTargetEx mCompositeHelper = new CompositionTargetEx();

			private Boolean mIsLoading = false;

			// Опорная сетка
			protected LineGeometry3D mGridGeometry;
			protected Color mGridColor;
			protected Transform3D mGridTransform;
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
			// ПАРАМЕТРЫ ВИЗУАЛИЗАЦИИ
			//
			/// <summary>
			/// Режим показа каркаса
			/// </summary>
			public Viewport3DX Helix3DViewer
			{
				get
				{
					return helix3DViewer;
				}
			}


			//
			// ПАРАМЕТРЫ ВИЗУАЛИЗАЦИИ
			//
			/// <summary>
			/// Режим показа каркаса
			/// </summary>
			public Boolean ShowWireframe
			{
				get
				{
					return mShowWireframe;
				}
				set
				{
					if (mShowWireframe != value)
					{
						mShowWireframe = value;
						NotifyPropertyChanged(PropertyArgsShowWireframe);

						foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
						{
							return node.IsRenderable;
						}))
						{
							if (node is MeshNode mesh)
							{
								mesh.RenderWireframe = value;
							}
						}
					}
				}
			}

			/// <summary>
			/// Режим закраски по Гуро
			/// </summary>
			public Boolean RenderFlat
			{
				get
				{
					return mRenderFlat;
				}
				set
				{
					if (mRenderFlat != value)
					{
						mRenderFlat = value;
						NotifyPropertyChanged(PropertyArgsRenderFlat);

						foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
						{
							return node.IsRenderable;
						}))
						{
							if (node is MeshNode mesh)
							{
								if (mesh.Material is PhongMaterialCore phong)
								{
									phong.EnableFlatShading = value;
								}
								else if (mesh.Material is PBRMaterialCore pbr)
								{
									pbr.EnableFlatShading = value;
								}
							}
						}
					}
				}
			}

			/// <summary>
			/// Ренденить карту окружения
			/// </summary>
			public Boolean RenderEnvironmentMap
			{
				get
				{
					return mRenderEnvironmentMap;
				}
				set
				{
					if (mRenderEnvironmentMap != value)
					{
						mRenderEnvironmentMap = value;
						NotifyPropertyChanged(PropertyArgsRenderEnvironmentMap);
						if (mSceneRoot != null)
						{
							foreach (var node in mSceneRoot.Traverse())
							{
								if (node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
								{
									material.RenderEnvironmentMap = value;
								}
							}
						}
					}
				}
			}

			/// <summary>
			/// Карта окружения
			/// </summary>
			public TextureModel EnvironmentMap
			{
				get
				{
					return mEnvironmentMap;
				}
			}

			//
			// КАМЕРА
			//
			/// <summary>
			/// Текущая модель камеры
			/// </summary>
			public String CameraModel
			{
				get { return mCameraModel; }
				set
				{
					if (mCameraModel != value)
					{
						mCameraModel = value;
						NotifyPropertyChanged(PropertyArgsCameraModel);
						RaiseCameraModelChanged();
					}
				}
			}

			/// <summary>
			/// Камера
			/// </summary>
			public Helix.Camera Camera
			{
				get
				{
					return mCamera;
				}

				protected set
				{
					mCamera = value;
					NotifyPropertyChanged(PropertyArgsCamera);
					CameraModel = value is Helix.PerspectiveCamera
										   ? PerspectiveCameraName
										   : value is Helix.OrthographicCamera ? OrthographicCameraName : null;
				}
			}

			/// <summary>
			/// Набор моделей камеры
			/// </summary>
			public List<String> CameraModelCollection { get; private set; }

			/// <summary>
			/// Событие смены камеры
			/// </summary>
			public event EventHandler CameraModelChanged;

			//
			// РЕНДЕР ТЕХНИКА
			//
			/// <summary>
			/// Менеджер эффектов
			/// </summary>
			public IEffectsManager EffectsManager
			{
				get { return mEffectsManager; }
				protected set
				{
					if (mEffectsManager != value)
					{
						mEffectsManager = value;
						NotifyPropertyChanged(PropertyArgsEffectsManager);
					}
				}
			}

			//
			// БАЗОВЫЕ МОДЕЛИ
			//
			/// <summary>
			/// Сцена
			/// </summary>
			public CScene3D Scene
			{
				get { return (mScene); }
				set
				{
					if (mScene != value)
					{
						mScene = value;
						NotifyPropertyChanged(PropertyArgsScene);
					}
				}
			}

			/// <summary>
			/// Корневой узел отображаемой сцены
			/// </summary>
			public SceneNode SceneRoot
			{
				get { return (mSceneRoot); }
				set
				{
					if (mSceneRoot != value)
					{
						mSceneRoot = value;
						NotifyPropertyChanged(PropertyArgsSceneRoot);
					}
				}
			}

			/// <summary>
			/// Все объекты сцены
			/// </summary>
			public Helix.SceneNodeGroupModel3D GroupModel
			{
				get { return (mGroupModel); }
				set
				{
					if (mGroupModel != value)
					{
						mGroupModel = value;
						NotifyPropertyChanged(PropertyArgsGroupModel);
					}
				}
			}

			/// <summary>
			/// Выбранная геометрия
			/// </summary>
			public Helix.GeometryModel3D SelectedModel
			{
				get { return (mSelectedModel); }
				set
				{
					if (mSelectedModel != value)
					{
						mSelectedModel = value;
						NotifyPropertyChanged(PropertyArgsSelectedModel);
					}
				}
			}

			//
			// АНИМАЦИЯ
			//
			/// <summary>
			/// Статус проигрывания анимации
			/// </summary>
			public Boolean EnableAnimation
			{
				get { return mEnableAnimation; }
				set
				{
					if (mEnableAnimation != value)
					{
						mEnableAnimation = value;
						NotifyPropertyChanged(PropertyArgsEnableAnimation);
						if (value)
						{
							StartAnimation();
						}
						else
						{
							StopAnimation();
						}
					}
				}
			}

			/// <summary>
			/// Список всех анимации сцены
			/// </summary>
			public IList<Animation> SceneAnimations
			{
				get { return mSceneAnimations; }
				set
				{
					mSceneAnimations = value;
				}
			}

			/// <summary>
			/// Список проигрываемых анимаций
			/// </summary>
			public ObservableCollection<IAnimationUpdater> Animations
			{
				get { return mAnimations; }
				set
				{
					mAnimations = value;
				}
			}

			/// <summary>
			/// Текущая выбранная анимация
			/// </summary>
			public IAnimationUpdater SelectedAnimation
			{
				get
				{
					return mSelectedAnimation;
				}
				set
				{
					if (mSelectedAnimation != value)
					{
						mSelectedAnimation = value;
						NotifyPropertyChanged(PropertyArgsSelectedAnimation);
						StopAnimation();
						if (value != null)
						{
							mAnimationUpdater = value;
							mAnimationUpdater.Reset();
							mAnimationUpdater.RepeatMode = AnimationRepeatMode.Loop;
							mAnimationUpdater.Speed = mSpeedAnimation;
						}
						else
						{
							mAnimationUpdater = null;
						}
						if (mEnableAnimation)
						{
							StartAnimation();
						}
					}
				}
			}

			/// <summary>
			/// Скорость воспроизведения анимации
			/// </summary>
			public Single SpeedAnimation
			{
				get
				{
					return mSpeedAnimation;
				}
				set
				{
					if (SpeedAnimation != value)
					{
						SpeedAnimation = value;
						NotifyPropertyChanged(PropertyArgsSpeedAnimation);
						if (mAnimationUpdater != null)
						{
							mAnimationUpdater.Speed = value;
						}
					}
				}
			}

			//
			// ОПОРНАЯ СЕТКА
			//
			/// <summary>
			/// Геометрия сетки
			/// </summary>
			public LineGeometry3D GridGeometry
			{
				get { return mGridGeometry; }
				set
				{
					if (mGridGeometry != value)
					{
						mGridGeometry = value;
						NotifyPropertyChanged(PropertyArgsGridGeometry);
					}
				}
			}

			/// <summary>
			/// Цвет линий сетки
			/// </summary>
			public Color GridColor
			{
				get { return mGridColor; }
				set
				{
					if (mGridColor != value)
					{
						mGridColor = value;
						NotifyPropertyChanged(PropertyArgsGridColor);
					}
				}
			}

			/// <summary>
			/// Трансформация сетки
			/// </summary>
			public Transform3D GridTransform
			{
				get { return mGridTransform; }
				set
				{
					if (mGridTransform != value)
					{
						mGridTransform = value;
						NotifyPropertyChanged(PropertyArgsGridTransform);
					}
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusViewerContent3D()
			{
				InitializeComponent();

				InitCamera();

				InitEffectsManager();

				InitGroupModel();

				InitAnimation();

				InitGrid();

				//threeViewer.AddHandler(Helix.Element3D.MouseDown3DEvent, new RoutedEventHandler((sender, args) =>
				//{
				//	var arg = args as Helix.MouseDown3DEventArgs;

				//	if (arg.HitTestResult == null)
				//	{
				//		return;
				//	}
				//	if (mSelectedModel != null)
				//	{
				//		mSelectedModel.PostEffects = null;
				//		mSelectedModel = null;
				//	}
				//	mSelectedModel = arg.HitTestResult.ModelHit as Helix.GeometryModel3D;
				//	if (mSelectedModel != null && mSelectedModel.Name != "gridBase")
				//	{
				//		mSelectedModel.PostEffects = String.IsNullOrEmpty(mSelectedModel.PostEffects) ? $"highlight[color:#FFFF00]" : null;
				//	}
				//}));
			}
			#endregion

			#region ======================================= МЕТОДЫ IDisposable ========================================
			/// <summary>
			/// To detect redundant calls
			/// </summary>
			private Boolean mDisposedValue = false;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			~LotusViewerContent3D()
			{
				// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(false);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Освобождение управляемых ресурсов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Dispose()
			{
				// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(true);
				// TODO: uncomment the following line if the finalizer is overridden above.
				// GC.SuppressFinalize(this);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Освобождение управляемых ресурсов
			/// </summary>
			/// <param name="disposing">Статус освобождения</param>
			//---------------------------------------------------------------------------------------------------------
			private void Dispose(Boolean disposing)
			{
				if (!mDisposedValue)
				{
					if (disposing)
					{
						// TODO: dispose managed state (managed objects).
					}

					// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
					// TODO: set large fields to null.
					if (EffectsManager != null)
					{
						var effectManager = EffectsManager as IDisposable;
						Disposer.RemoveAndDispose(ref effectManager);
					}
					mDisposedValue = true;
					GC.SuppressFinalize(this);
				}
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение камеры
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseCameraModelChanged()
			{
				CameraModelChanged?.Invoke(this, new EventArgs());
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
				Assimp.PostProcessSteps post_process_steps = Assimp.PostProcessSteps.None;
				TreeView tree_view_model_structure = null;

				// Если файл пустой то используем диалог
				if (String.IsNullOrEmpty(file_name))
				{
					file_name = XFileDialog.Open("Открыть файл", "", OpenFileFilter);
					if (file_name.IsExists())
					{
						if(parameters_open != null)
						{
							post_process_steps = parameters_open.GetValueOfType<Assimp.PostProcessSteps>(Assimp.PostProcessSteps.None);
							tree_view_model_structure = parameters_open.GetValueOfType<TreeView>();
						}

						// Загружаем файл
						Load(file_name, post_process_steps, tree_view_model_structure);

						FileName = file_name;
						XLogger.LogInfoModule(nameof(LotusViewerContent3D), $"Открыт файл с именем: [{FileName}]");
					}
				}
				else
				{
					if (parameters_open != null)
					{
						post_process_steps = parameters_open.GetValueOfType<Assimp.PostProcessSteps>(Assimp.PostProcessSteps.None);
						tree_view_model_structure = parameters_open.GetValueOfType<TreeView>();
					}

					// Загружаем файл
					Load(file_name, post_process_steps, tree_view_model_structure);

					FileName = file_name;
					XLogger.LogInfoModule(nameof(LotusViewerContent3D), $"Открыт файл с именем: [{FileName}]");
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

			}
			#endregion

			#region ======================================= МЕТОДЫ ИНИЦИАЛИЗАЦИИ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных камеры
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitCamera()
			{
				mDefaultOrthographicCamera = new Helix.OrthographicCamera
				{
					Position = new Point3D(0, 0, 5),
					LookDirection = new Vector3D(-0, -0, -5),
					UpDirection = new Vector3D(0, 1, 0),
					NearPlaneDistance = 0.05,
					FarPlaneDistance = 5000
				};

				mDefaultPerspectiveCamera = new Helix.PerspectiveCamera
				{
					Position = new Point3D(0, 0, 5),
					LookDirection = new Vector3D(-0, -0, -5),
					UpDirection = new Vector3D(0, 1, 0),
					NearPlaneDistance = 0.05,
					FarPlaneDistance = 5000
				};

				// camera models
				CameraModelCollection = new List<String>()
				{
					OrthographicCameraName,
					PerspectiveCameraName,
				};

				// on camera changed callback
				CameraModelChanged += (sender, args) =>
				{
					if (mCameraModel == OrthographicCameraName)
					{
						if (!(Camera is Helix.OrthographicCamera))
							Camera = mDefaultOrthographicCamera;
					}
					else if (mCameraModel == PerspectiveCameraName)
					{
						if (!(Camera is Helix.PerspectiveCamera))
							Camera = mDefaultPerspectiveCamera;
					}
					else
					{
						//throw new Helix3D.("Camera Model Error.");
					}
				};

				// default camera model
				CameraModel = PerspectiveCameraName;
				Camera = mDefaultPerspectiveCamera;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных рендер техники
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitEffectsManager()
			{
				EffectsManager = new DefaultEffectsManager();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных базовой модели
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitGroupModel()
			{
				mGroupModel = new Helix.SceneNodeGroupModel3D();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных анимации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitAnimation()
			{
				mAnimations = new ObservableCollection<IAnimationUpdater>();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных опорной сетки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitGrid()
			{
				// floor plane grid
				GridGeometry = LineBuilder.GenerateGrid(new SharpDX.Vector3(0, 1, 0), 0, 10);
				GridColor = Colors.DarkGray;
				GridTransform = new TranslateTransform3D(-5, 0, -5);
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка 3D контента из файла
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			/// <param name="post_process_steps">Флаги обработки контента</param>
			/// <param name="tree_view_model_structure">Дерево для просмотра внутренней структуры 3D контента</param>
			//---------------------------------------------------------------------------------------------------------
			public void Load(String file_name, Assimp.PostProcessSteps post_process_steps, TreeView tree_view_model_structure)
			{
				if (mIsLoading)
				{
					return;
				}

				StopAnimation();

				mIsLoading = true;

				// Загружаем в отдельной задачи
				Task.Run(() =>
				{
					var loader = new Importer();

					if (post_process_steps != Assimp.PostProcessSteps.None)
					{
						loader.Configuration.AssimpPostProcessSteps = post_process_steps;
					}

					return loader.Load(file_name);
				}).ContinueWith((result) =>
				{
					mIsLoading = false;
					if (result.IsCompleted)
					{
						HelixToolkitScene helix_toolkit_scene = result.Result;
						if (helix_toolkit_scene == null) return;
						mSceneRoot = helix_toolkit_scene.Root;
						mSceneAnimations = helix_toolkit_scene.Animations;
						Animations.Clear();
						GroupModel.Clear();
						if (helix_toolkit_scene != null)
						{
							if (helix_toolkit_scene.Root != null)
							{
								foreach (var node in helix_toolkit_scene.Root.Traverse())
								{
									if (node is MaterialGeometryNode m)
									{
										if (m.Material is PBRMaterialCore pbr)
										{
											pbr.RenderEnvironmentMap = RenderEnvironmentMap;
										}
										else if (m.Material is PhongMaterialCore phong)
										{
											phong.RenderEnvironmentMap = RenderEnvironmentMap;
										}
									}
								}
							}

							GroupModel.AddNode(helix_toolkit_scene.Root);
							if (helix_toolkit_scene.HasAnimation)
							{
								var dict = helix_toolkit_scene.Animations.CreateAnimationUpdaters();
								foreach (var animation in dict.Values)
								{
									Animations.Add(animation);
								}
							}
							foreach (var node in helix_toolkit_scene.Root.Traverse())
							{
								//node.Tag = new AttachedNodeViewModel(node);
							}
						}

						if(tree_view_model_structure != null)
						{
							//tree_view_model_structure.ItemsSource = mSceneRoot.Items;
						}

					}
					else if (result.IsFaulted && result.Exception != null)
					{
						MessageBox.Show(result.Exception.Message);
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Старт анимации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void StartAnimation()
			{
				mCompositeHelper.Rendering += CompositeHelper_Rendering;
			}

			/// <summary>
			/// Остановка анимации
			/// </summary>
			public void StopAnimation()
			{
				mCompositeHelper.Rendering -= CompositeHelper_Rendering;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление анимации
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void CompositeHelper_Rendering(Object sender, RenderingEventArgs args)
			{
				if (mAnimationUpdater != null)
				{
					mAnimationUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
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