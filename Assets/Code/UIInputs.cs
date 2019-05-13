﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class UIInputs : MonoBehaviour
    {
        public World world;
        bool leftClickDown = false;
        bool rightClickDown = false;
        public float scrollSpeed = 0.4f;

        public void Update()
        {
            if (world.map == null) { return; }
            mouseClicks();
            scrollKeys();
            scaling();
            hotkeys();
        }


        public void hotkeys()
        {
            //if (world.ui.blocker != null) { return; }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                //world.uiHex.cheatField.gameObject.SetActive(!world.uiHex.cheatField.gameObject.activeInHierarchy);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Tab))
            {
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
            }

            if (world.ui.state == UIMaster.uiState.WORLD || world.ui.state == UIMaster.uiState.BACKGROUND)
            {
                if (Input.GetKey(KeyCode.Alpha0))
                {
                    world.map.masker.mask = MapMaskManager.maskType.NONE;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.NATION)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        World.log("Set to nation");
                        world.map.masker.mask = MapMaskManager.maskType.NATION;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.PROVINCE)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.PROVINCE;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.TESTING)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.TESTING;
                    }
                }
            }
            GraphicalMap.checkData();
        }

        public void scaling()
        {
            if (world.ui.state == UIMaster.uiState.WORLD)
            {
                if (Input.GetKeyDown("z") || Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.scale = GraphicalMap.scale * 1.1f;
                    if (GraphicalMap.scale > GraphicalMap.maxScale) { GraphicalMap.scale = GraphicalMap.maxScale; }
                    GraphicalMap.checkData();
                }
                else if (Input.GetKeyDown("x") || Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.scale = GraphicalMap.scale / 1.1f;
                    if (GraphicalMap.scale < GraphicalMap.minScale) { GraphicalMap.scale = GraphicalMap.minScale; }
                    GraphicalMap.checkData();
                }
            }
        }
        public void scrollKeys()
        {

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - 5)
            {
                //GraphicalMap.panStepsToTake = 0;
                GraphicalMap.lastMapChange += 1;
                GraphicalMap.y -= scrollSpeed;
                if (GraphicalMap.y < 0) { GraphicalMap.y = 0; }
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.mousePosition.y <= 2)
            {
                //GraphicalMap.panStepsToTake = 0;
                GraphicalMap.lastMapChange += 1;
                GraphicalMap.y += scrollSpeed;
                if (GraphicalMap.y > world.map.sy) { GraphicalMap.y = world.map.sy; }
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.mousePosition.x <= 0)
            {
                //GraphicalMap.panStepsToTake = 0;
                GraphicalMap.lastMapChange += 1;
                GraphicalMap.x += scrollSpeed;
                if (GraphicalMap.x > world.map.sx) { GraphicalMap.x = world.map.sx; }
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - 5)
            {
                //GraphicalMap.panStepsToTake = 0;
                GraphicalMap.lastMapChange += 1;
                GraphicalMap.x -= scrollSpeed;
                if (GraphicalMap.x < 0) { GraphicalMap.x = 0; }
            }
        }

        public void mouseClicks()
        {
            if (world.ui.blocker != null) { return; }//Blocker on screen takes precedence

            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    return;
                }
            }

            if (Input.GetMouseButtonDown(0) && !leftClickDown) { leftClick(); }
            else if (Input.GetMouseButtonDown(1) && !rightClickDown) { rightClick(); }
            leftClickDown = Input.GetMouseButtonDown(0);
            rightClickDown = Input.GetMouseButtonDown(1);
        }

        public void leftClick()
        {
            if (world.ui.state == UIMaster.uiState.WORLD)
            {
                clickOnHex();
            }
            else
            {
                //if (GraphicalSociety.personOver != null)
                //{
                //    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                //    {
                //        GraphicalSociety.slotClickedNation(GraphicalSociety.personOver);
                //        world.soundSource.select();
                //        return;
                //    }
                //    GraphicalSociety.slotClicked(GraphicalSociety.personOver);
                //    world.soundSource.select();
                //}
            }
        }

        public void rightClick()
        {
            if (world.ui.state == UIMaster.uiState.WORLD)
            {
                rightClickOnHex();
            }

        }

        public void clickOnHex()
        {
            Hex clickedHex = GraphicalMap.getHexUnderMouse(Input.mousePosition).hex;
            bool deselectedProperty = false;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                GraphicalMap.selectedProperty = null;
                GraphicalMap.selectedHex = clickedHex;
                return;
            }


            if (GraphicalMap.selectedProperty != null)
            {
                //If we've clicked on his hex we want to cycle to the next unit in the cycle
                if (GraphicalMap.selectedProperty.getLoc().hex == clickedHex)
                {
                    int index = GraphicalMap.selectedProperty.getLoc().getProperties().IndexOf(GraphicalMap.selectedProperty);
                    bool foundAny = false;
                    for (int i = index + 1; i < clickedHex.location.getProperties().Count; i++)
                    {
                        foundAny = true;
                        GraphicalMap.selectedProperty = clickedHex.location.getProperties()[i];
                        //world.ui.uiUnit.setTo(GraphicalMap.selectedProperty);
                        GraphicalMap.selectedHex = null;
                        return;
                    }

                    //Found no further units, want to select hex instead
                    if (!foundAny)
                    {
                        deselectedProperty = true;
                        GraphicalMap.selectedProperty = null;
                    }
                }
                else
                {
                    GraphicalMap.selectedProperty = null;
                }
            }

            if (GraphicalMap.selectedProperty == null && !deselectedProperty)
            {
                //See if there's someone to select
                if (clickedHex.location != null)
                {
                    foreach (Property u in clickedHex.location.getProperties())
                    {
                        GraphicalMap.selectedProperty = u;
                        //world.ui.uiUnit.setTo(GraphicalMap.selectedProperty);
                        GraphicalMap.selectedHex = null;
                        return;

                    }
                }
            }

            GraphicalMap.selectedHex = clickedHex;
            world.ui.uiHex.checkData();
        }

        public void rightClickOnHex()
        {
            GraphicalMap.checkData();
        }
    }
}