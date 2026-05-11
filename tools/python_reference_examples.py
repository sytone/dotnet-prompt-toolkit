#!/usr/bin/env python3
"""Reference prompt_toolkit scenario outputs used by tools/validate_examples.py."""

from __future__ import annotations

import json
import os
import sys
from typing import Any

try:
    from prompt_toolkit.buffer import Buffer
    from prompt_toolkit.completion import CompleteEvent, WordCompleter
    from prompt_toolkit.document import Document
    from prompt_toolkit.formatted_text import FormattedText, fragment_list_to_text
    from prompt_toolkit.history import InMemoryHistory
except ModuleNotFoundError as exc:  # pragma: no cover - exercised by users without deps installed.
    print(
        "prompt_toolkit is not installed. Run: python -m pip install -r tools/python-requirements.txt",
        file=sys.stderr,
    )
    raise SystemExit(2) from exc


def document_scenario() -> dict[str, Any]:
    document = Document("alpha\nbeta gamma", cursor_position=12)
    return {
        "scenario": "document",
        "text": document.text,
        "cursorPosition": document.cursor_position,
        "row": document.cursor_position_row,
        "column": document.cursor_position_col,
        "currentLineBeforeCursor": document.current_line_before_cursor,
        "wordBeforeCursor": document.get_word_before_cursor(),
    }


def completion_scenario() -> dict[str, Any]:
    document = Document("he", cursor_position=2)
    completer = WordCompleter(["help", "hello", "world"])
    completions = sorted(
        completer.get_completions(document, CompleteEvent()),
        key=lambda completion: completion.text,
    )
    return {
        "scenario": "completion",
        "completions": [
            {
                "text": completion.text,
                "startPosition": completion.start_position,
                "applied": document.text[: document.cursor_position + completion.start_position]
                + completion.text
                + document.text_after_cursor,
            }
            for completion in completions
        ],
    }


def history_scenario() -> dict[str, Any]:
    history = InMemoryHistory()
    buffer = Buffer(history=history)
    buffer.insert_text("abc")
    history.append_string(buffer.text)
    return {"scenario": "history", "entries": list(history.get_strings())}


def formatted_text_scenario() -> dict[str, Any]:
    fragments = FormattedText([("class:greeting", "hello"), ("", " world")])
    return {
        "scenario": "formatted-text",
        "plainText": fragment_list_to_text(fragments),
        "fragments": [{"style": style, "text": text} for style, text, *_ in fragments],
    }


def layout_scenario() -> dict[str, Any]:
    return {"scenario": "layout", "plainText": os.linesep.join(["dotnet-prompt-toolkit", "> "])}


def run_scenario(scenario: str) -> dict[str, Any]:
    scenarios = {
        "document": document_scenario,
        "completion": completion_scenario,
        "history": history_scenario,
        "formatted-text": formatted_text_scenario,
        "layout": layout_scenario,
    }
    if scenario == "all":
        return {"scenario": "all", "results": [factory() for factory in scenarios.values()]}
    try:
        return scenarios[scenario]()
    except KeyError as exc:
        raise SystemExit(f"Unknown parity scenario '{scenario}'.") from exc


if __name__ == "__main__":
    selected = sys.argv[1] if len(sys.argv) > 1 else "all"
    print(json.dumps(run_scenario(selected), sort_keys=True, separators=(",", ":")))
