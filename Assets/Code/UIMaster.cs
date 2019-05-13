﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIMaster : MonoBehaviour
    {
        public World world;
        public UIHex uiHex;
       // public UICity uiCity;
        //public UIWorldRight uiWorldRight;
        //public UISociety uiSociety;
       // public UIMainMenu uiMainMenu;
        //public UICityFullscreen uiCityFullscreen;
        public GameObject uiMaster;

        public List<GameObject> blockerQueue;
        public List<GameObject> blockerQueueDelayed = new List<GameObject>();
        public GameObject blocker;
        public GameObject hexSelector;
        //public ViewSelector_Person viewSelector;

        //public List<Alert> alertQueue = new List<Alert>();

        public enum uiState { SELECT_SOC, SOCIETY, WORLD, BACKGROUND, MAIN_MENU };
        public uiState state = uiState.MAIN_MENU;

        public void Update()
        {
            if (world.map != null)
            {
                if (GraphicalMap.selectedHex == null || GraphicalMap.selectedHex.outer == null)
                {
                    hexSelector.SetActive(false);
                }
                else
                {
                    hexSelector.SetActive(true);
                    hexSelector.transform.localScale = new Vector3(GraphicalMap.scale, GraphicalMap.scale, 1);
                    hexSelector.transform.localPosition = GraphicalMap.getLoc(GraphicalMap.selectedHex) + new Vector3(0, 0, -5);
                }

                if (state == uiState.WORLD)
                {
                    GraphicalMap.tick();
                    if (GraphicalMap.selectedHex == null)
                    {
                        //uiCity.gameObject.SetActive(false);
                        uiHex.gameObject.SetActive(true);
                    }
                    else
                    {
                       uiHex.gameObject.SetActive(true);
                    }
                }
                else if (state == uiState.SOCIETY || state == uiState.SELECT_SOC)
                {
                    //GraphicalSociety.tick();
                }
            }
            else
            {

                hexSelector.SetActive(false);
            }
            checkBlockerQueue();
        }


        public void bEndTurn()
        {
            if (world.turnLock) { return; }
            if (blocker != null) { return; }

           // if (alertQueue.Count > 0) { bViewAlerts(); return; }


            world.turnLock = true;
            world.map.turnTick();
            world.turnLock = false;

            if (state == uiState.SELECT_SOC || state == uiState.SOCIETY)
            {
                //GraphicalSociety.checkData();
                //uiSociety.setTo(GraphicalSociety.focal);
            }
            else if (state == uiState.WORLD)
            {
                GraphicalMap.checkData();
            }
        }

        /*
        public void bViewSociety()
        {
            if (GraphicalMap.selectedHex == null) { return; }
            if (GraphicalMap.selectedHex.owner == null) { return; }
            if (GraphicalMap.selectedHex.owner is Society == false) { return; }
            if (GraphicalMap.selectedHex.settlement != null)
            {
                if (GraphicalMap.selectedHex.settlement is Set_City)
                {
                    Set_City city = (Set_City)GraphicalMap.selectedHex.settlement;
                    if (city.lordSlot != null)
                    {
                        setToSociety(city.lordSlot.society, city.lordSlot);
                        return;
                    }
                }
            }
            setToSociety((Society)GraphicalMap.selectedHex.owner);
        }
        */

        /*
        public void setToSociety(Society soc)
        {
            state = uiState.SOCIETY;
            uiWorldRight.gameObject.SetActive(false);
            uiHex.gameObject.SetActive(false);
            uiMainMenu.gameObject.SetActive(false);
            uiSociety.gameObject.SetActive(true);
            uiCommon.gameObject.SetActive(true);
            uiCity.gameObject.SetActive(false);
            GraphicalMap.purge();

            GraphicalSociety.centreOn(soc.command.top);
            uiSociety.setTo(soc.command.top.person);
        }
        public void setToSociety(Society soc, Slot slot)
        {
            state = uiState.SOCIETY;
            uiWorldRight.gameObject.SetActive(false);
            uiHex.gameObject.SetActive(false);
            uiMainMenu.gameObject.SetActive(false);
            uiSociety.gameObject.SetActive(true);
            uiCommon.gameObject.SetActive(true);
            uiCity.gameObject.SetActive(false);
            GraphicalMap.purge();

            GraphicalSociety.centreOn(slot);
            uiSociety.setTo(slot.person);
        }
        */

        public void setToBackground()
        {

            state = uiState.BACKGROUND;
            //uiWorldRight.gameObject.SetActive(false);
            uiHex.gameObject.SetActive(false);
            //uiMainMenu.gameObject.SetActive(false);
            //uiSociety.gameObject.SetActive(false);
            uiMaster.gameObject.SetActive(false);
            //uiCity.gameObject.SetActive(false);
            GraphicalMap.purge();
            //GraphicalSociety.purge();
        }

        public void setToWorld()
        {

            state = uiState.WORLD;
            //uiWorldRight.gameObject.SetActive(true);
            uiHex.gameObject.SetActive(true);
            //uiMainMenu.gameObject.SetActive(false);
            //uiSociety.gameObject.SetActive(false);
            uiMaster.gameObject.SetActive(true);
            //uiCity.gameObject.SetActive(false);

            //GraphicalSociety.purge();
        }
        public void setToMainMenu()
        {

            state = uiState.MAIN_MENU;
            //uiWorldRight.gameObject.SetActive(false);
            uiHex.gameObject.SetActive(false);
            //uiMainMenu.gameObject.SetActive(true);
            //uiSociety.gameObject.SetActive(false);
            uiMaster.gameObject.SetActive(false);
            //uiCity.gameObject.SetActive(false);

            //GraphicalSociety.purge();
            GraphicalMap.purge();
        }

        //public void bViewWorld()
        //{
        //    setToWorld();

        //    if (GraphicalSociety.focal != null && GraphicalSociety.focal.slot != null) { GraphicalSociety.focalSlot = GraphicalSociety.focal.slot; }
        //    if (GraphicalSociety.focalSlot != null)
        //    {
        //        if (GraphicalSociety.focalSlot.domain != null)
        //        {
        //            Hex hex = GraphicalSociety.focalSlot.domain.loc.hex;
        //            GraphicalMap.panTo(hex.x, hex.y);
        //        }
        //    }
        //}

        //public void bWorldMap()
        //{
        //    if (world.turnLock) { return; }
        //    if (blocker != null) { return; }
        //    setToBackground();

        //    PopupWorldMap worldMap = world.prefabStore.getWorldMap(world, world.map);
        //    addBlocker(worldMap.gameObject);
        //}
        //public void bPlayback()
        //{
        //    if (world.turnLock) { return; }
        //    if (blocker != null) { return; }
        //    setToBackground();

        //    PopupPlayback worldMap = world.prefabStore.getPlayback(world, world.map);
        //    addBlockerDontHide(worldMap.gameObject);
        //}

        //public void addAlert(Vote v, Person p, Society soc, Hex hex, string title, string msg, string lockout)
        //{
        //    if (world.displayMessages == false) { return; }
        //    if (world.map.permaDismissed.Contains(lockout)) { return; }

        //    Alert a = new Alert();
        //    a.targetVote = v;
        //    a.targetHex = hex;
        //    a.targetPerson = p;
        //    a.targetSociety = soc;
        //    a.title = title;
        //    a.words = msg;
        //    a.lockout = lockout;
        //    //alertQueue.Add(a);
        //    Alert alert = a;
        //    world.prefabStore.popupAlert(alert.title, alert.words, alert.targetVote, alert.targetSociety, alert.targetPerson, alert.targetHex, alert.lockout);

        //}

        public void addBlocker(GameObject block)
        {
            //if (CheatMenu.nopopups)
            //{
            //    Destroy(block);
            //    return;
            //}

            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueue.Add(block);
                block.SetActive(false);//Hide all non-main items
            }
        }
        public void addBlockerToDelayedQueue(GameObject block)
        {
            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueueDelayed.Add(block);
                block.SetActive(false);//Hide all non-main items
            }
        }
        public void addBlockerToQueueFrontDontHide(GameObject block)
        {
            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueue.Insert(0, block);
            }
        }

        public void addBlockerDontHide(GameObject block)
        {
            //if (CheatMenu.nopopups)
            //{
            //    Destroy(block);
            //    return;
            //}

            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueue.Add(block);
            }
        }

        public void checkBlockerQueue()
        {
            if (blocker == null)
            {
                if (blockerQueue.Count > 0)
                {
                    GameObject next = blockerQueue[0];
                    blockerQueue.RemoveAt(0);

                    next.SetActive(true);//Show new item from queue
                    blocker = next;
                    return;
                }
                if (blockerQueueDelayed.Count > 0)
                {
                    GameObject next = blockerQueueDelayed[0];
                    blockerQueueDelayed.RemoveAt(0);

                    next.SetActive(true);//Show new item from queue
                    blocker = next;
                    return;
                }
            }
        }

        public void removeBlocker(GameObject block)
        {
            if (blocker == block)
            {
                if (blockerQueue.Count > 0)
                {
                    GameObject next = blockerQueue[0];
                    blockerQueue.RemoveAt(0);

                    next.SetActive(true);//Show new item from queue
                    blocker = next;
                }
                DestroyImmediate(block);
            }
            else if (blockerQueue.Contains(block))
            {
                blockerQueue.Remove(block);
                DestroyImmediate(block);
            }

            if (state == uiState.BACKGROUND)
            {
                setToWorld();
            }
            else if (state == uiState.WORLD)
            {
                GraphicalMap.checkData();
            }
            else
            {
                //GraphicalSociety.checkData();
            }
        }

        public void bMainMenu()
        {
            this.setToMainMenu();
        }

        //public void bViewAlerts()
        //{
        //    if (world.turnLock) { return; }//Can't act while turn is advancing
        //    if (blocker != null) { return; }//Can't take action with blocker onscreen
        //    if (alertQueue.Count == 0) { return; }
        //    Alert alert = alertQueue[0];
        //    alertQueue.RemoveAt(0);
        //    world.soundSource.singleNoteWood();
        //    world.prefabStore.popupAlert(alert.title, alert.words, alert.targetVote, alert.targetSociety, alert.targetPerson, alert.targetHex, alert.lockout);
        //}


        //private void findAgentSoc()
        //{
        //    List<Person> opts = new List<Person>();
        //    foreach (SocialGroup group in world.map.socialGroups)
        //    {
        //        if (group is Society)
        //        {
        //            Society soc = (Society)group;

        //            foreach (Person p in soc.people)
        //            {
        //                if (p.enthralled)
        //                {
        //                    opts.Add(p);
        //                }
        //            }
        //        }
        //    }

        //    if (opts.Count == 0)
        //    {
        //        return;
        //    }
        //    if (GraphicalSociety.focal != null && opts.Contains(GraphicalSociety.focal))
        //    {
        //        int ind = opts.IndexOf(GraphicalSociety.focal);
        //        setToSociety(opts[ind + 1].society, opts[ind + 1].slot);
        //        return;
        //    }
        //    else
        //    {
        //        setToSociety(opts[0].society, opts[0].slot);
        //    }
        //}


        //private void findNobleOrFail()
        //{
        //    List<Person> opts = new List<Person>();
        //    foreach (SocialGroup group in world.map.socialGroups)
        //    {
        //        if (group is Society)
        //        {
        //            Society soc = (Society)group;

        //            foreach (Person p in soc.people)
        //            {
        //                if (p.enthralled)
        //                {
        //                    opts.Add(p);
        //                }
        //            }
        //        }
        //    }
        //    if (opts.Count == 0)
        //    {
        //        world.prefabStore.popMsg("No enthralled (noble or agent) could be found.");
        //        world.soundSource.failure();
        //        return;
        //    }

        //    //Found someone. Set to them
        //    setToSociety(opts[0].society, opts[0].slot);
        //}
    }
}