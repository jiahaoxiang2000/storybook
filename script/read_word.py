#!/usr/bin/env python3
"""
Script to read DOCX file and extract text and images.
Reads from ~/Downloads/PROG2006_Ass2_Report_lishiya.docx
Creates a markdown file with text and image references.
"""

import os
import re
from pathlib import Path

from docx import Document
from docx.oxml import CT_P, CT_Tbl
from docx.oxml.text.paragraph import CT_P
from docx.table import Table, _Cell
from docx.text.paragraph import Paragraph


def iter_block_items(parent):
    """
    Generate a reference to each paragraph and table child within parent,
    in document order. Yields tuples of (paragraph, None) or (None, table).
    """
    from docx.oxml.xmlchemy import BaseOxmlElement

    if hasattr(parent, "element"):
        parent_elm = parent.element.body
    else:
        parent_elm = parent

    for child in parent_elm.iterchildren():
        if isinstance(child, CT_P):
            yield Paragraph(child, parent), None
        elif isinstance(child, CT_Tbl):
            yield None, Table(child, parent)


def extract_text_and_images(
    docx_path: str,
    output_dir: str = "extracted",
    md_output: str = "extracted_content.md",
):
    """
    Extract all text and images from a DOCX file and save to markdown.

    Args:
        docx_path: Path to the DOCX file
        output_dir: Directory to save extracted images
        md_output: Path to save the markdown file
    """
    # Expand user path
    docx_path = os.path.expanduser(docx_path)

    # Create output directory for images
    output_path = Path(output_dir)
    output_path.mkdir(parents=True, exist_ok=True)

    # Load the document
    doc = Document(docx_path)

    print(f"Reading document: {docx_path}\n")
    print("=" * 80)
    print("EXTRACTING CONTENT TO MARKDOWN")
    print("=" * 80)

    # First, extract all images and create a mapping from rId to filename
    image_map = {}
    image_count = 0

    for rel in doc.part.rels.values():
        if "image" in rel.target_ref:
            image_count += 1
            # Get image data
            image = rel.target_part.blob

            # Determine file extension from content type
            content_type = rel.target_part.content_type
            ext_map = {
                "image/png": ".png",
                "image/jpeg": ".jpg",
                "image/jpg": ".jpg",
                "image/gif": ".gif",
                "image/bmp": ".bmp",
                "image/tiff": ".tiff",
            }
            ext = ext_map.get(content_type, ".png")

            # Save image
            image_filename = f"image_{image_count}{ext}"
            image_path = output_path / image_filename

            with open(image_path, "wb") as f:
                f.write(image)

            # Store the relationship ID to filename mapping
            image_map[rel.rId] = image_filename
            print(f"Saved: {image_filename} ({len(image)} bytes)")

    print(f"\nTotal images extracted: {image_count}")
    print(f"Images saved to: {output_path.absolute()}")

    # Now create markdown with text and image references
    markdown_content = []
    markdown_content.append(f"# Extracted Content from Document\n")
    markdown_content.append(f"Source: {docx_path}\n")
    markdown_content.append(f"---\n\n")

    # Process document in order (paragraphs and tables)
    for para, table in iter_block_items(doc):
        if para is not None:
            # Check if paragraph contains images
            for run in para.runs:
                if "graphic" in run._element.xml:
                    # Extract image references from the run
                    for blip in run._element.xpath(".//a:blip"):
                        embed = blip.get(
                            "{http://schemas.openxmlformats.org/officeDocument/2006/relationships}embed"
                        )
                        if embed and embed in image_map:
                            image_filename = image_map[embed]
                            markdown_content.append(
                                f"![{image_filename}]({output_dir}/{image_filename})\n\n"
                            )
                            print(f"Image reference added: {image_filename}")

            # Add paragraph text
            if para.text.strip():
                # Check if it's a heading based on style
                style_name = para.style.name if para.style else ""
                if "Heading 1" in style_name:
                    markdown_content.append(f"# {para.text}\n\n")
                elif "Heading 2" in style_name:
                    markdown_content.append(f"## {para.text}\n\n")
                elif "Heading 3" in style_name:
                    markdown_content.append(f"### {para.text}\n\n")
                else:
                    markdown_content.append(f"{para.text}\n\n")

        elif table is not None:
            # Format table as markdown
            markdown_content.append("\n")
            for row_idx, row in enumerate(table.rows):
                row_text = " | ".join(cell.text.strip() for cell in row.cells)
                if row_text.strip():
                    markdown_content.append(f"| {row_text} |\n")
                    # Add header separator after first row
                    if row_idx == 0:
                        num_cells = len(row.cells)
                        markdown_content.append(
                            f"| {' | '.join(['---'] * num_cells)} |\n"
                        )
            markdown_content.append("\n")

    # Write markdown file
    md_path = Path(md_output)
    with open(md_path, "w", encoding="utf-8") as f:
        f.writelines(markdown_content)

    print(f"\nMarkdown file created: {md_path.absolute()}")
    print("=" * 80)
    print("EXTRACTION COMPLETE")
    print("=" * 80)


if __name__ == "__main__":
    # Path to the document
    docx_file = "~/Downloads/PROG2006_Ass2_Report_wangnan.docx"

    # Output directory for images (relative to current directory)
    output_directory = "extracted"

    try:
        extract_text_and_images(docx_file, output_directory)
    except FileNotFoundError:
        print(f"Error: File not found at {os.path.expanduser(docx_file)}")
        print("Please check the file path and try again.")
    except Exception as e:
        print(f"Error: {e}")
