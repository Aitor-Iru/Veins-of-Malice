# Roadmap Técnico: Veins of Malice (Vertical Slice)

**Rol:** Lead Game Designer / Product Manager  
**Objetivo:** Guía de implementación granular desde concepto hasta Vertical Slice.

Este documento sirve como hoja de ruta viva para el desarrollo. Marcar las casillas `[x]` a medida que se completen las tareas.

## Fase 1: Pre-Producción y Configuración (Semanas 1-2)

### 1.1. Configuración del Proyecto (Unity)
- [x] **Repositorio y Control de Versiones**
    - [x] Inicializar repositorio Git con `.gitignore` adecuado para Unity.
    - [x] **Workflow Simplificado:** Trabajar directamente en rama `main` (Solo Dev).
    - [x] Definir estructura de carpetas (Scripts, Prefabs, Art, Audio, Scenes).
- [ ] **Definición de Pipeline de Arte (2.5D)**
    - [x] Configurar URP (Universal Render Pipeline) con soporte para Lit Shaders.
    - [x] Establecer estándares de importación para Modelos 3D (.fbx, .blend) y Materiales.
    - [x] Configurar Culling Masks y Layers para separar Gameplay (3D) de UI (Overlay).
- [x] **Prototipo de "Greybox"**
    - [x] Crear escena de prueba con geometría básica (cubos/placeholders). *(GreyboxSceneBuilder.cs — Tools > Veins of Malice > Create Greybox Scene)*
    - [x] Implementar cámara 2D básica (CameraController.cs con seguimiento suave y dead zone).
    - [x] Dash cooldown (1.5s), Coyote Time (0.15s) y Jump Buffer (0.1s) en PlayerController.
    - [x] PlayerHealth.cs con invulnerabilidad temporal.

### 1.2. Arquitectura del Código
- [x] **Game Manager**
    - [x] Implementar patrón Singleton para `GameManager`.
    - [x] Crear máquina de estados global (MainMenu, Gameplay, Pause, GameOver).
    - [x] Eventos estáticos: `OnGameStateChanged`, `OnGamePaused`, `OnGameResumed`, `OnGameOver`.
    - [x] Gestión de escenas: `StartGame`, `RestartGame`, `GoToMainMenu`, `QuitGame`.
    - [x] Pausa con tecla `Escape` (toggle).
    - [x] Integración con `PlayerHealth.OnDeath` → dispara GameOver.
- [x] **Input System**
    - [x] Configurar Unity Input System (New) — asset `InputSystem_Actions.inputactions`.
    - [x] Mapear acciones: Move, Jump, Attack, Dash, Interact, Pause.
    - [x] Bindings: Teclado (WASD/flechas, Space, Shift, Escape, E, LMB) + Gamepad.
    - [x] `InputReader.cs` (ScriptableObject) — centraliza eventos de input, desacopla sistemas.
    - [x] `PlayerController.cs` actualizado para usar `InputReader` (sin `PlayerInput` directo).

---

## Fase 2: Core Gameplay - "Los Cimientos" (Semanas 3-6)

### 2.1. Controlador del Personaje (Kairo)
- [x] **Movimiento Básico**
    - [x] Implementar movimiento horizontal con física (Rigidbody).
    - [x] Implementar salto y doble salto (jumpsRemaining logic).
    - [x] Implementar Dash (con cooldown y duración).
- [x] **Sistema de Combate (Base)**
    - [x] Hitboxes y Hurtboxes: Detección via OverlapSphere e interfaz IDamageable.
    - [x] Ataque Básico: Combo de 3 golpes (lógica de ventana de input).
    - [x] Bloqueo/Parry: Reducción de daño (70%) implementada en PlayerHealth.
- [ ] **Animación (Mecanim 3D)**
    - [ ] Configurar Animator Controller para el Modelo 3D (Humanoid/Generic).
    - [ ] Implementar estados de ataque y blending de animaciones (Idle -> Run -> Attack).

### 2.2. Sistema de Enemigos (IA Básica)
- [ ] **Arquitectura de IA**
    - [x] Definir Jerarquía de Enemigos (Grados 4 a 1) y Roles.
    - [ ] Crear clase base `Enemy` (hereda de `MonoBehaviour` o clase `Entity`).
    - [ ] Implementar Máquina de Estados Finita (FSM): Patrulla, Persecución, Ataque, Muerte.
- [ ] **Maldición Grado 4 (Masilla)**
    - [ ] Implementar comportamiento simple (caminar hacia el jugador, atacar en rango).
    - [ ] Hit detection y feedback visual (parpadeo al recibir daño).

### 2.3. UI y Feedback (HUD)
- [ ] **HUD Básico**
    - [ ] Barra de Vida (Health Bar) con animación de pérdida de vida.
    - [ ] Barra de Energía Maldita (Cursed Energy).
- [ ] **Feedback de Combate**
    - [ ] Números de daño flotantes (Floating Text).
    - [ ] Screen Shake sutil al golpear.

---

## Fase 3: Producción - Vertical Slice "Barrio Deformado" (Semanas 7-12)

### 3.1. Diseño de Nivel y Arte (Estilo Clash Royale)
- [ ] **Escenario: Barrio Deformado (3D)**
    - [ ] Importar assets 3D optimizados (Low Poly, Toon Shader).
    - [ ] Implementar iluminación en tiempo real (Directional Light, Point Lights para efectos mágicos).
    - [ ] Colocar props 3D y obstáculos destructibles con físicas.
- [ ] **Arte de Personajes (3D)**
    - [ ] Reemplazar placeholders de Kairo con modelo 3D final y texturas PBR/Toon.
    - [ ] Integrar modelos de Enemigo Grado 4 y 3 con Materiales Lit.
- [ ] **VFX (Efectos Visuales)**
    - [ ] Partículas de "Esencia Maldita" al golpear.
    - [ ] Efecto de "Dash" (rastro o fantasma).

### 3.2. Sistemas RPG y Narrativa
- [ ] **Sistema de Diálogo**
    - [ ] Crear sistema de cajas de texto (NPCs).
    - [ ] Integrar un NPC aliado (ej. Rion o Mentor) con diálogo ramificado simple.
- [ ] **Progresión (Vertical Slice Scope)**
    - [ ] Drop de "Esencia Maldita" al matar enemigos.
    - [ ] Menú simple de mejora de habilidades (Mockup funcional).
- [ ] **Puntos de Guardado**
    - [ ] Implementar "Santuarios" o puntos de control que guarden estado básico (Vida, Posición).

### 3.3. Enemigo Elite / Mini-Boss
- [ ] **Maldición Grado 2 (Mini-Boss)**
    - [ ] Diseñar patrón de ataque de 2 fases.
    - [ ] Fase 1: Ataques físicos pesados.
    - [ ] Fase 2: Ataque de área o proyectil de energía.
    - [ ] Barra de vida de jefe en UI.

---

## Fase 4: Polish y Cierre de Vertical Slice (Semanas 13-14)

### 4.1. Audio
- [ ] **SFX (Efectos de Sonido)**
    - [ ] Pasos (cambian según superficie).
    - [ ] Impactos de combate y voces de esfuerzo (Grr, Ha!).
    - [ ] Feedback de UI (Hover, Click).
- [ ] **Banda Sonora y Ambience**
    - [ ] Loop de música de exploración (Tenso/Misterioso).
    - [ ] Loop de música de combate (Dinámico si es posible, o transición por trigger).

### 4.2. Menús y Flujo de Juego
- [ ] **Menú Principal**
    - [ ] Botones: Jugar, Opciones, Salir.
    - [ ] Arte de fondo animado.
- [ ] **Game Loop Completo**
    - [ ] Inicio -> Tutorial/Intro -> Gameplay -> Boss -> Pantalla de "Gracias por jugar".
    - [ ] Pantalla de Pause funcional.
    - [ ] Pantalla de Game Over con opción de "Reintentar".

### 4.3. Optimización y QA
- [ ] **Performance**
    - [ ] Verificar framerate estable (60 FPS).
    - [ ] Optimizar texturas y audios (compresión).
- [ ] **Bug Fixing**
    - [ ] Playtesting intensivo de colisiones y combate.
    - [ ] Ajustar "Coyote Time" y "Jump Buffer" para mejorar el feel del salto.

---

## Checklist de Validación (Definition of Done) para Vertical Slice

- [ ] El jugador puede moverse, saltar y dashear fluidamente.
- [ ] El combate se siente responsivo (golpes, hits, muerte de enemigos).
- [ ] Hay un principio y un final claro en la demo.
- [ ] La UI comunica claramente el estado del jugador (Vida/Energía).
- [ ] No existen bugs bloqueantes (softlocks o crashes).
- [ ] El arte y el audio están implementados y son coherentes con la visión "Dark Stylized".
