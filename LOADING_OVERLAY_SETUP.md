# Loading Overlay Setup Guide

## Что это
Overlay с темным фоном и анимацией бегуна (512x512), показывается при телепорте камеры на дальние расстояния (>50 тайлов) для маскировки загрузки тайлов.

## Структура UI в Unity

### 1. Создать отдельный Canvas для Overlay (ВАЖНО!)
- Hierarchy → Right Click → UI → Canvas
- **Переименовать**: `LoadingOverlayCanvas`
- **Canvas**:
  - Render Mode: **Screen Space - Overlay**
  - Sort Order: **1000** (чтобы быть поверх всех UI элементов)
- **Canvas Scaler**: 
  - UI Scale Mode: Scale With Screen Size
  - Reference Resolution: 1920x1080

### 2. Создать LoadingOverlay
```
LoadingOverlayCanvas (Canvas, Sort Order = 1000)
  └── LoadingOverlay (GameObject)
      ├── Background (Image - полупрозрачный черный фон, растянут на весь экран)
      └── RunnerImage (Image - 512x512 по центру с анимацией)
```

### 3. Настройка Background
- **Component**: Image
- **Color**: Black (0, 0, 0, 0.8) - полупрозрачный темный
- **Raycast Target**: **✓ ВКЛЮЧЕН** (блокирует клики под overlay)
- **RectTransform**:
  - Anchors: Stretch (Left: 0, Top: 0, Right: 0, Bottom: 0)
  - Width/Height: 0 (заполняет весь экран)

### 4. Настройка RunnerImage
- **Component**: Image
- **Source Image**: Любой кадр из `runner_run_sheet.png` (например, runner_run_sheet_0)
- **Raycast Target**: **✗ ВЫКЛЮЧЕН** (не блокирует клики, только фон блокирует)
- **RectTransform**:
  - Anchors: Center (0.5, 0.5)
  - Width: 512, Height: 512
  - Position: (0, 0, 0)
- **Animator**:
  - Add Component → Animator
  - Controller: `runner_run.controller` (из Assets/Art/Characters/LoadingRunner/)
  - Update Mode: **Unscaled Time** (чтобы анимация играла даже при Time.timeScale = 0)

### 5. Настройка LoadingOverlayController
- **На LoadingOverlay GameObject**:
  - Add Component → LoadingOverlayController
  - Background Panel: перетащить Background (Image)
  - Runner Image: перетащить RunnerImage (Image)
  - Runner Animator: перетащить RunnerImage → Animator
  - Background Color: (0, 0, 0, 0.8)

### 6. Подключить к MapUIController
- Найти в сцене объект с компонентом `MapUIController`
- В Inspector:
  - Loading Overlay: перетащить LoadingOverlay GameObject

### 7. Выключить LoadingOverlay по умолчанию
- В Hierarchy выбрать LoadingOverlay
- Снять галочку рядом с именем (GameObject.SetActive = false)
- **LoadingOverlayCanvas** остается активным!

## Важные моменты

### Почему отдельный Canvas?
- **Sort Order = 1000** гарантирует что overlay будет поверх всех других UI (карта, кнопки, etc)
- **Screen Space - Overlay** режим рендерит поверх всего в сцене
- Изолирует loading overlay от основного UI

### Raycast Target
- **Background**: включен → блокирует все клики под собой
- **RunnerImage**: выключен → не мешает Background'у

1. **Игрок кликает "Center Camera"** → `MapUIController.OnCenterCamera()`
2. **Проверка расстояния**:
   - Если < 50 тайлов → плавное движение камеры (без overlay)
   - Если > 50 тайлов → телепорт с overlay
3. **При телепорте**:
   - `loadingOverlay.Show()` → показать темный фон + запустить анимацию бегуна
   - `CameraMaster.TeleportToPlayer()` → мгновенный телепорт камеры
   - `yield return new WaitForEndOfFrame()` → ждать окончания рендера (тайлы прогрузятся)
   - `loadingOverlay.Hide()` → скрыть overlay

## Файлы

- **Script**: `Assets/Scripts/UI/LoadingOverlayController.cs`
- **Animation Clip**: `Assets/Art/Characters/LoadingRunner/runner_run.anim`
- **Animator Controller**: `Assets/Art/Characters/LoadingRunner/runner_run.controller`
- **Spritesheet**: `Assets/Art/Characters/LoadingRunner/runner_run_sheet.png`

## Troubleshooting

**Анимация не играет:**
- Проверь что Animator.controller = runner_run.controller
- Проверь что в Animation window клип runner_run содержит кадры
- Убедись что Animator.enabled = true при Show()

**Overlay не показывается:**
- Проверь что LoadingOverlay.SetActive(false) в начале (Awake)
- Проверь что MapUIController.loadingOverlay не null
- Проверь что расстояние > 50 тайлов (или уменьши teleportDistanceThreshold)

**Темный фон не на весь экран:**
- Background → RectTransform → Anchors: Stretch all
- Background → Left/Top/Right/Bottom = 0

**Бегун не по центру:**
- RunnerImage → RectTransform → Anchors: Center (0.5, 0.5)
- RunnerImage → Position: (0, 0, 0)
