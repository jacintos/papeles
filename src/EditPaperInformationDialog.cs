/* -*- coding: utf-8 -*- */
/* EditPaperInformationDialog.cs
 * Copyright (c) 2009 Jacinto Shy, Jr.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

using Gtk;
using System.IO;

namespace Papeles
{
	public class EditPaperInformationDialog
	{
		[Glade.Widget] Dialog edit_paper_information_dialog;
		[Glade.Widget] VBox edit_paper_information_dialog_vbox;
		[Glade.Widget] Label paper_file_name_label;

		Entry title_entry;
		Entry authors_entry;
		Entry cite_key_entry;
		Entry uri_entry;
		Entry doi_entry;
		Entry journal_name_entry;
		Entry journal_volume_entry;
		Entry journal_number_entry;
		Entry journal_pages_entry;
		Entry journal_year_entry;
		TextView abstract_view;

		Widget CreateNotebookBasicWidget (Paper paper)
		{
			Alignment alignment = new Alignment (0f, 0f, 1f, 1f);
			VBox box = new VBox ();
			VBox innerBox;
			Label titleLabel   = new Label ("Title:");
			Label authorsLabel = new Label ("Authors:");
			Label citeKeyLabel = new Label ("Cite Key:");
			Label uriLabel     = new Label ("URI:");
			Label doiLabel     = new Label ("DOI:");

			titleLabel.Xalign   = 0f;
			authorsLabel.Xalign = 0f;
			citeKeyLabel.Xalign = 0f;
			uriLabel.Xalign     = 0f;
			doiLabel.Xalign     = 0f;

			title_entry    = new Entry (paper.Title   != null ? paper.Title   : "");
			authors_entry  = new Entry (paper.Authors != null ? paper.Authors : "");
			cite_key_entry = new Entry (paper.CiteKey != null ? paper.CiteKey : "");
			uri_entry      = new Entry (paper.Uri     != null ? paper.Uri     : "");
			doi_entry      = new Entry (paper.Doi     != null ? paper.Doi     : "");

			innerBox = new VBox ();
			innerBox.Add (titleLabel);
			innerBox.Add (title_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (authorsLabel);
			innerBox.Add (authors_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (citeKeyLabel);
			innerBox.Add (cite_key_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (uriLabel);
			innerBox.Add (uri_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (doiLabel);
			innerBox.Add (doi_entry);
			box.Add (innerBox);

			box.Spacing = 6;
			alignment.TopPadding  = alignment.BottomPadding = 4;
			alignment.LeftPadding = alignment.RightPadding  = 6;
			alignment.Add (box);

			return alignment;
		}

		Widget CreateNotebookJournalWidget (Paper paper)
		{
			Alignment alignment = new Alignment (0f, 0f, 1f, 1f);
			VBox box = new VBox ();
			VBox innerBox;
			Label nameLabel   = new Label ("Name:");
			Label volumeLabel = new Label ("Volume:");
			Label numberLabel = new Label ("Number:");
			Label pagesLabel  = new Label ("Pages:");
			Label yearLabel   = new Label ("Year:");

			nameLabel.Xalign   = 0f;
			volumeLabel.Xalign = 0f;
			numberLabel.Xalign = 0f;
			pagesLabel.Xalign  = 0f;
			yearLabel.Xalign   = 0f;

			journal_name_entry   = new Entry (paper.Journal != null ? paper.Journal : "");
			journal_volume_entry = new Entry (paper.Volume  != null ? paper.Volume  : "");
			journal_number_entry = new Entry (paper.Number  != null ? paper.Number  : "");
			journal_pages_entry  = new Entry (paper.Pages   != null ? paper.Pages   : "");
			journal_year_entry   = new Entry (paper.Year    != null ? paper.Year    : "");

			innerBox = new VBox ();
			innerBox.Add (nameLabel);
			innerBox.Add (journal_name_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (volumeLabel);
			innerBox.Add (journal_volume_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (numberLabel);
			innerBox.Add (journal_number_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (pagesLabel);
			innerBox.Add (journal_pages_entry);
			box.Add (innerBox);

			innerBox = new VBox ();
			innerBox.Add (yearLabel);
			innerBox.Add (journal_year_entry);
			box.Add (innerBox);

			box.Spacing = 6;
			alignment.TopPadding  = alignment.BottomPadding = 4;
			alignment.LeftPadding = alignment.RightPadding  = 6;
			alignment.Add (box);

			return alignment;
		}

		Widget CreateNotebookAbstractWidget (Paper paper)
		{
			Alignment alignment = new Alignment (0f, 0f, 1f, 1f);
			ScrolledWindow scWin = new ScrolledWindow ();

			abstract_view = new TextView ();

			if (paper.Abstract != null) {
				TextBuffer buffer = abstract_view.Buffer;
				buffer.Text = paper.Abstract;
			}
			scWin.Add (abstract_view);

			alignment.TopPadding  = alignment.BottomPadding = 6;
			alignment.LeftPadding = alignment.RightPadding  = 6;
			alignment.Add (scWin);

			return alignment;
		}

		void SaveEditedInformation (Paper paper)
		{
			paper.Title    = title_entry.Text;
			paper.Authors  = authors_entry.Text;
			paper.CiteKey  = cite_key_entry.Text;
			paper.Uri      = uri_entry.Text;
			paper.Doi      = doi_entry.Text;
			paper.Journal  = journal_name_entry.Text;
			paper.Volume   = journal_volume_entry.Text;
			paper.Number   = journal_number_entry.Text;
			paper.Pages    = journal_pages_entry.Text;
			paper.Year     = journal_year_entry.Text;
			paper.Abstract = abstract_view.Buffer.Text;

			paper.Save ();
		}

		public EditPaperInformationDialog (Paper paper)
		{
			Glade.XML gxml = new Glade.XML (null, "papeles.glade", "edit_paper_information_dialog", null);

			gxml.Autoconnect (this);

			Notebook notebook = new Notebook ();
			notebook.AppendPage (CreateNotebookBasicWidget (paper),   new Label ("Basic"));
			notebook.AppendPage (CreateNotebookJournalWidget (paper), new Label ("Journal"));
			notebook.AppendPage (CreateNotebookAbstractWidget (paper), new Label ("Abstract"));

			paper_file_name_label.Text = Path.GetFileName (paper.FilePath);

			edit_paper_information_dialog_vbox.Add (notebook);
			edit_paper_information_dialog.ShowAll ();

			if (edit_paper_information_dialog.Run () == 2)
				SaveEditedInformation (paper);

			edit_paper_information_dialog.Destroy ();
		}
	}
}
