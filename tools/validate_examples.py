#!/usr/bin/env python3
"""Run repeatable Python-vs-C# example parity validation scenarios."""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
EXAMPLE_PROJECT = ROOT / "examples" / "DotnetPromptToolkit.Examples" / "DotnetPromptToolkit.Examples.csproj"
PYTHON_REFERENCE = ROOT / "tools" / "python_reference_examples.py"
SCENARIOS = ("document", "completion", "history", "formatted-text", "layout")


def run(command: list[str]) -> subprocess.CompletedProcess[str]:
    return subprocess.run(command, cwd=ROOT, check=False, text=True, capture_output=True)


def parse_json_output(label: str, completed: subprocess.CompletedProcess[str]) -> dict[str, Any]:
    if completed.returncode != 0:
        raise RuntimeError(
            f"{label} failed with exit code {completed.returncode}\n"
            f"STDOUT:\n{completed.stdout}\nSTDERR:\n{completed.stderr}"
        )
    try:
        return json.loads(completed.stdout)
    except json.JSONDecodeError as exc:
        raise RuntimeError(f"{label} did not emit valid JSON. Output:\n{completed.stdout}") from exc


def validate_scenario(scenario: str, configuration: str) -> None:
    python_result = parse_json_output(
        f"Python scenario {scenario}",
        run([sys.executable, str(PYTHON_REFERENCE), scenario]),
    )
    csharp_result = parse_json_output(
        f"C# scenario {scenario}",
        run(
            [
                "dotnet",
                "run",
                "--project",
                str(EXAMPLE_PROJECT),
                "--configuration",
                configuration,
                "--no-build",
                "--",
                "parity",
                scenario,
            ]
        ),
    )
    if python_result != csharp_result:
        raise AssertionError(
            f"Scenario '{scenario}' did not match.\n"
            f"Python: {json.dumps(python_result, indent=2, sort_keys=True)}\n"
            f"C#:     {json.dumps(csharp_result, indent=2, sort_keys=True)}"
        )


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--iterations", type=int, default=1, help="Number of full validation passes to run.")
    parser.add_argument("--configuration", default="Release", choices=("Debug", "Release"))
    parser.add_argument("--scenario", choices=SCENARIOS, action="append", help="Scenario to run; defaults to all.")
    parser.add_argument("--no-build", action="store_true", help="Skip the initial dotnet build.")
    args = parser.parse_args()

    if args.iterations < 1:
        parser.error("--iterations must be >= 1")

    if not args.no_build:
        build = run(["dotnet", "build", "DotnetPromptToolkit.slnx", "--configuration", args.configuration])
        if build.returncode != 0:
            print(build.stdout, end="")
            print(build.stderr, end="", file=sys.stderr)
            return build.returncode

    scenarios = tuple(args.scenario or SCENARIOS)
    for iteration in range(1, args.iterations + 1):
        for scenario in scenarios:
            validate_scenario(scenario, args.configuration)
        print(f"iteration {iteration}: validated {', '.join(scenarios)}")

    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:  # noqa: BLE001 - command-line tool should print a concise failure.
        print(f"validation failed: {exc}", file=sys.stderr)
        raise SystemExit(1) from exc
