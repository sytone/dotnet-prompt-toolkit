#!/usr/bin/env python3
"""Reference prompt_toolkit scenario outputs used by tools/validate_examples.py.

For each parity scenario emitted by examples/DotnetPromptToolkit.Examples, this
module produces the equivalent Python output so the validator can compare the
two byte-for-byte.
"""

from __future__ import annotations

import json
import os
import re
import sys
from typing import Any

try:
    from prompt_toolkit.auto_suggest import AutoSuggestFromHistory
    from prompt_toolkit.buffer import Buffer
    from prompt_toolkit.completion import CompleteEvent, FuzzyWordCompleter, NestedCompleter, WordCompleter
    from prompt_toolkit.document import Document
    from prompt_toolkit.formatted_text import ANSI, FormattedText, HTML, fragment_list_to_text
    from prompt_toolkit.history import InMemoryHistory
    from prompt_toolkit.styles.named_colors import NAMED_COLORS
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


def fuzzy_completion_scenario() -> dict[str, Any]:
    document = Document("ab", cursor_position=2)
    completer = FuzzyWordCompleter(["abc", "axb", "zzz"])
    texts = [c.text for c in completer.get_completions(document, CompleteEvent())]
    return {"scenario": "fuzzy-completion", "completions": sorted(set(texts) & {"abc", "axb"})}


def nested_completion_scenario() -> dict[str, Any]:
    nested = NestedCompleter.from_nested_dict(
        {"show": {"users": None, "roles": None}, "help": None}
    )
    document = Document("show u", cursor_position=6)
    texts = sorted(c.text for c in nested.get_completions(document, CompleteEvent()))
    return {"scenario": "nested-completion", "completions": texts}


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


def html_scenario() -> dict[str, Any]:
    fragments = list(HTML("<b>bold</b> and <i>italic</i>").formatted_text)
    return {
        "scenario": "html",
        "plainText": fragment_list_to_text(fragments),
        "fragments": [{"style": style, "text": text} for style, text, *_ in fragments],
    }


def ansi_scenario() -> dict[str, Any]:
    fragments = list(ANSI("\x1b[31mred\x1b[0m plain")._formatted_text)
    return {"scenario": "ansi", "plainText": fragment_list_to_text(fragments)}


def layout_scenario() -> dict[str, Any]:
    return {"scenario": "layout", "plainText": os.linesep.join(["dotnet-prompt-toolkit", "> "])}


def search_scenario() -> dict[str, Any]:
    text = "hello world hello"
    needle = "hello"
    forward_index = text.find(needle, 6)
    backward_index = text.rfind(needle, 0, 12 - 1)
    return {
        "scenario": "search",
        "forwardIndex": forward_index if forward_index >= 0 else None,
        "forwardLength": len(needle) if forward_index >= 0 else None,
        "backwardIndex": backward_index if backward_index >= 0 else None,
        "backwardLength": len(needle) if backward_index >= 0 else None,
    }


def auto_suggest_scenario() -> dict[str, Any]:
    history = InMemoryHistory()
    history.append_string("hello world")
    suggest = AutoSuggestFromHistory()
    document = Document("hello", cursor_position=5)
    # AutoSuggestFromHistory needs a buffer; reuse the Buffer API.
    buffer = Buffer(history=history, document=document)
    suggestion = suggest.get_suggestion(buffer, document)
    return {"scenario": "auto-suggest", "text": suggestion.text if suggestion else None}


def named_colors_scenario() -> dict[str, Any]:
    def lookup(name: str) -> str | None:
        if name.startswith("#"):
            return name
        for key, hex_value in NAMED_COLORS.items():
            if key.lower() == name.lower():
                return hex_value.lower()
        return None
    return {
        "scenario": "named-colors",
        "red": lookup("red"),
        "blue": lookup("blue"),
        "custom": lookup("#abcdef"),
    }


def grammar_scenario() -> dict[str, Any]:
    pattern = re.compile(r"(?P<command>\w+)\s+(?P<argument>\w+)")
    match = pattern.match("show users")
    assert match is not None
    return {
        "scenario": "grammar",
        "command": match.group("command"),
        "argument": match.group("argument"),
    }


def progress_bar_scenario() -> dict[str, Any]:
    total, current, width = 4, 2, 8
    ratio = current / total
    filled = round(ratio * width)
    bar = "█" * filled + "░" * (width - filled)
    rendered = f"|{bar}| {current:>4}/{total} ({ratio:.0%})"
    return {"scenario": "progress-bar", "rendered": rendered}


def frame_scenario() -> dict[str, Any]:
    title = "title"
    body = "body"
    width = max(len(title), len(body)) + 2
    top_label = f"┤ {title} ├".ljust(width, "─")
    top = "┌" + top_label + "┐"
    inside = "│ " + body.ljust(width - 2) + " │"
    bottom = "└" + ("─" * width) + "┘"
    return {"scenario": "frame", "rendered": "\n".join([top, inside, bottom])}


SCENARIOS = {
    "document": document_scenario,
    "completion": completion_scenario,
    "fuzzy-completion": fuzzy_completion_scenario,
    "nested-completion": nested_completion_scenario,
    "history": history_scenario,
    "formatted-text": formatted_text_scenario,
    "html": html_scenario,
    "ansi": ansi_scenario,
    "layout": layout_scenario,
    "search": search_scenario,
    "auto-suggest": auto_suggest_scenario,
    "named-colors": named_colors_scenario,
    "grammar": grammar_scenario,
    "progress-bar": progress_bar_scenario,
    "frame": frame_scenario,
}


def run_scenario(scenario: str) -> dict[str, Any]:
    if scenario == "all":
        return {"scenario": "all", "results": [factory() for factory in SCENARIOS.values()]}
    try:
        return SCENARIOS[scenario]()
    except KeyError as exc:
        raise SystemExit(f"Unknown parity scenario '{scenario}'.") from exc


if __name__ == "__main__":
    selected = sys.argv[1] if len(sys.argv) > 1 else "all"
    print(json.dumps(run_scenario(selected), sort_keys=True, separators=(",", ":")))
