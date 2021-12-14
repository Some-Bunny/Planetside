using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
/*

namespace Planetside
{

    public class LevelerGUIController : BraveBehaviour
    {
        readonly string spriteDir = "Planetside/Resources/VFX/KnuckleBlaster";
        Transform m_extantGUI;
        tk2dSprite m_panel;
        tk2dSprite[] m_icons;
        PlayerController m_player;
        Vector2
            targetScale = Vector2.zero,
            baseScale = new Vector2(1, 1);
        int selectedStat = 0;
        public KnuckleBlaster m_item;
        public static Shader holoShader = ShaderCache.Acquire("Brave/Internal/HologramShader");
        static float lum = 5;
        Color
            lumWhite = new Color(lum, lum, lum),
            lumGrey = new Color(.5f, .5f, .5f),
            lumGreen = new Color(0, lum, 0),
            lumMagenta = new Color(lum, 0, lum);

        public void Build(KnuckleBlaster item, PlayerController player)
        {
            if (m_extantGUI != null) return;
            this.m_item = item;
            this.m_player = player;
            m_extantGUI = SpriteBuilder.SpriteFromResource($"{spriteDir}/panel").transform;
            m_panel = m_extantGUI.gameObject.GetComponent<tk2dSprite>();
            m_panel.transform.parent = m_player.transform;
            m_panel.SortingOrder = 0;
            m_panel.IsPerpendicular = false;


            shown = false;
            m_panel.scale = Vector2.zero;
            Invoke("InitializeAppearance", .1f);
        }

        Vector3 xpBarOffset = new Vector3(9.5f, 19f, -16f);
        Vector3 readMeOffset = new Vector3(58f, 7f, -16f);
        Vector3 pointsOffset = new Vector3(22f, 6f, -16f);

        Vector3 iconsOffset = new Vector3(23f, 20, -1);
        Vector3 levelsOffset = new Vector3(22, 37, 1);
        

        tk2dSprite CreateSpriteElement(string name, Vector3 offset)
        {
            var sprite = SpriteBuilder.SpriteFromResource($"{spriteDir}/{name}").GetComponent<tk2dSprite>();
            sprite.transform.SetParent(m_extantGUI);
            sprite.PlaceAtLocalPositionByAnchor(offset / 16f, tk2dBaseSprite.Anchor.LowerLeft);
            sprite.scale = Vector2.zero;
            sprite.SortingOrder = 1;
            sprite.IsPerpendicular = false;
            m_panel.AttachRenderer(sprite);
            return sprite;
        }


        Vector2 scale;
        void FixedUpdate()
        {
            if (!shown)
            {
                if (ShrinkComponents())
                {
                    scale = m_panel.scale;
                    if (Vector2.Distance(scale, targetScale) < .05f)
                        m_panel.scale = targetScale;
                    else
                        m_panel.scale = Vector2.Lerp(scale, targetScale, .2f);
                }
            }
            else
            {
                float dist = Vector2.Distance(scale, targetScale);
                scale = m_panel.scale;
                if (dist < .05f)
                {
                    m_panel.scale = targetScale;
                    ShrinkComponents();
                }
                else
                    m_panel.scale = Vector2.Lerp(scale, targetScale, .2f);
            }
            m_panel.PlaceAtPositionByAnchor(m_player.primaryHand.sprite.WorldCenter.ToVector3ZUp() + new Vector3(0, 0, 10f), tk2dBaseSprite.Anchor.MiddleCenter);

        }


        public bool ShrinkComponents()
        {
            float dist = Vector2.Distance(scale, targetScale);
            if (Vector2.Distance(scale, targetScale) < .05f)
            {
                if (dist != 0)
                return true;
            }
            return false;

        }




        bool locked = true;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
                Toggle();
            if (Key(GungeonActions.GungeonActionType.Reload) && KeyTime(GungeonActions.GungeonActionType.Reload) > .5f && !locked)
            {
                Toggle();
                locked = true;
            }

            if (!Key(GungeonActions.GungeonActionType.Reload))
                locked = false;

            if (shown)
            {
                if (KeyDown(GungeonActions.GungeonActionType.SelectLeft))
                    //MoveCursor(-1);
                if (KeyDown(GungeonActions.GungeonActionType.SelectRight))
                    //MoveCursor(1);
                if (KeyDown(GungeonActions.GungeonActionType.Interact))
                    AddLevel();
            }
        }

        void AddLevel()
        {
            //m_item.AddLevel(selectedStat);
            //UpdateLevels();
        }


        void MoveCursor(int dir)
        {
            selectedStat = Mathf.Clamp(selectedStat + dir, 0, KnuckleBlaster.FistToUse.Count - 1);
            for (int i = 0; i < m_icons.Length; i++)
                m_icons[i].SetColor((i == selectedStat) ? lumMagenta : lumWhite);
            AkSoundEngine.PostEvent("Play_UI_menu_select_01", m_player.gameObject);

        }

        void InitializeAppearance()
        {
            //m_panel.renderer.material.shader = holoShader;
            foreach (var p in m_panel.GetComponentsInChildren<tk2dSprite>())
                if (p != m_panel)
                    p.SetColor(lumWhite);

            for (int i = 0; i < m_icons.Length; i++)
                m_icons[i].SetColor((i == selectedStat) ? lumMagenta : lumWhite);

        }

        bool shown;
        void Toggle()
        {
            shown = !shown;
            if (shown)
            {
                targetScale = baseScale;
                //m_player.SetInputOverride("Leveler");
            }
            else
            {
                //m_player.ClearInputOverride("Leveler");
                targetScale = Vector2.zero;
            }
        }

        public void Destroy()
        {
            Destroy(m_extantGUI);
            Destroy(this);
        }

        public float KeyTime(GungeonActions.GungeonActionType action)
        {
            return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
        }

        public bool KeyDown(GungeonActions.GungeonActionType action)
        {
            return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
        }

        public bool Key(GungeonActions.GungeonActionType action)
        {
            return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
        }
    }
}
*/