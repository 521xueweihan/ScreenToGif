﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ScreenToGif.Util.Writers;

namespace ScreenToGif.Util
{
    public static class Clipboard
    {
        #region Properties

        private static string Folder { get; set; }

        private static string CurrentFolder { get; set; }

        public static List<List<FrameInfo>> Items { get; private set; } = new List<List<FrameInfo>>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies the selected frames to a Clipboard folder.
        /// </summary>
        /// <param name="target">The selected frames.</param>
        /// <returns>The selected copied frames.</returns>
        public static bool Copy(List<FrameInfo> target)
        {
            try
            {
                PrepareCurrent(target[0].ImageLocation);

                var newList = new List<FrameInfo>();

                foreach (var frameInfo in target)
                {
                    //Changes the path of the image.
                    var filename = Path.Combine(CurrentFolder, Path.GetFileName(frameInfo.ImageLocation));

                    //Copy the image to the folder.
                    File.Copy(frameInfo.ImageLocation, filename, true);

                    //Create the new object and add to the list.
                    newList.Add(new FrameInfo(filename, frameInfo.Delay, frameInfo.CursorInfo));
                }

                //Adds the current copied list to the clipboard.
                Items.Add(newList);
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex, "Copy to Clipboard");
                return false;
            }
        }

        /// <summary>
        /// Cuts the selected frames to a Clipboard folder.
        /// </summary>
        /// <param name="target">The selected frames.</param>
        /// <returns>The selected cut frames.</returns>
        public static bool Cut(List<FrameInfo> target)
        {
            try
            {
                PrepareCurrent(target[0].ImageLocation);

                var newList = new List<FrameInfo>();

                foreach (var frameInfo in target)
                {
                    //Changes the path of the image.
                    var filename = Path.Combine(CurrentFolder, Path.GetFileName(frameInfo.ImageLocation));

                    //Copy the image to the folder.
                    File.Copy(frameInfo.ImageLocation, filename, true);
                    File.Delete(frameInfo.ImageLocation);

                    //Create the new object and add to the list.
                    newList.Add(new FrameInfo(filename, frameInfo.Delay, frameInfo.CursorInfo));
                }

                //Adds the current cut list to the clipboard.
                Items.Add(newList);
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex, "Cut to Clipboard");
                return false;
            }
        }

        /// <summary>
        /// Copies the selected frames to a Clipboard folder.
        /// </summary>
        /// <returns>The selected copied/cut frames.</returns>
        public static List<FrameInfo> Paste(int index, int pasteIndex)
        {
            var newList = new List<FrameInfo>();
            var recordingFolder = Path.GetDirectoryName(Items[index][0].ImageLocation);

            foreach (var frameInfo in Items[index])
            {
                //Changes the path of the image.
                var filename = Path.Combine(recordingFolder,
                    $"{pasteIndex} - {Path.GetFileNameWithoutExtension(frameInfo.ImageLocation)} {DateTime.Now.ToString("hh-mm-ss-FFFF")}.png");

                //Copy the image to the folder.
                File.Copy(frameInfo.ImageLocation, filename, true);

                //Create the new object and add to the list.
                newList.Add(new FrameInfo(filename, frameInfo.Delay, frameInfo.CursorInfo));
            }

            return newList;
        }

        /// <summary>
        /// Removes the item from the clipboard.
        /// </summary>
        /// <param name="index">The index to be removed.</param>
        public static void Remove(int index)
        {
            foreach (var frameInfo in Items[index])
            {
                //Copy the image to the folder.
                File.Delete(frameInfo.ImageLocation);
            }

            Items.RemoveAt(index);
        }

        #endregion

        #region Private Methods

        private static void Prepare(string imageLocation)
        {
            Folder = Path.Combine(Path.GetDirectoryName(imageLocation), "Clipboard");

            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
        }

        private static void PrepareCurrent(string imageLocation)
        {
            Prepare(imageLocation);

            CurrentFolder = Path.Combine(Folder, Items.Count.ToString());

            if (!Directory.Exists(CurrentFolder))
                Directory.CreateDirectory(CurrentFolder);
        }

        #endregion
    }
}
