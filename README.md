# faketasks

Terminal-friendly fake task generator for demos, live streams, and screenshots. `faketasks` is inspired by the legendary [genact](https://github.com/svenstaro/genact) and recreates the feel of busy consoles using .NET 8 and [Spectre.Console](https://spectreconsole.net/cli).

## Table of Contents
- [Features](#features)
- [Requirements](#requirements)
- [Quick Start](#quick-start)
- [Usage](#usage)
  - [Bootlog module](#bootlog-module)
  - [Utility commands](#utility-commands)
- [Configuration Reference](#configuration-reference)
- [Project Layout](#project-layout)
- [Embedded Data](#embedded-data)
- [Development](#development)
- [Roadmap](#roadmap)
- [License](#license)

## Features
- Linux boot log simulation with kernel, hardware discovery, filesystem, and systemd phases
- Pluggable module scheduler that enforces time/count limits and speed scaling
- Rich CLI UX powered by Spectre.Console tables, colors, and markup
- Embedded JSON datasets so the app runs offline without additional assets

## Requirements
- .NET SDK 8.0 or newer
- Windows, macOS, or Linux terminal with ANSI color support

## Quick Start
```bash
git clone https://github.com/Ddemon26/faketasks.git
cd faketasks
dotnet build
dotnet run --project faketasks.Cli -- list-modules
```

## Usage
### Bootlog module
Simulate Linux kernel boot messages. By default the scheduler loops indefinitely until you stop it.

```bash
dotnet run --project faketasks.Cli -- bootlog
```

Run a short, faster demo:

```bash
dotnet run --project faketasks.Cli -- bootlog --test --speed 2.0 --instant 5
```

Sample output:

```text
[   0.000102] Linux version 5.12.9-generic
[   0.000311] Command line: BOOT_IMAGE=/boot/vmlinuz root=UUID=...
[   0.000523] smpboot: CPU12 CPUs detected
[   0.000781] eth0: link up, 1000 Mbps, MAC 12:ab:cd:34:ef:56
[   0.001043] systemd[1]: Started NetworkManager.service
Boot completed in 1.2s
```

### Utility commands
- `list-modules`: show available and planned modules
- `version`: print assembly metadata, runtime, and project URL

```bash
dotnet run --project faketasks.Cli -- list-modules
dotnet run --project faketasks.Cli -- version
```

## Configuration Reference
| Option | Description |
| --- | --- |
| `-t`, `--test` | Run a single module iteration and exit |
| `-s`, `--speed <factor>` | Scale delays (values > 1 speed up output) |
| `--instant <lines>` | Print the first N lines without delay |
| `--count <count>` | Exit after running N modules |
| `--time <seconds>` | Exit after the specified duration |

Options can be combined, for example `faketasks bootlog --time 30 --speed 1.5`.

## Project Layout
```
faketasks.sln
├─ faketasks.Cli       # Spectre.Console CLI host and command bindings
└─ faketasks.Core      # Modules, orchestration, helpers, and embedded data
```

Key building blocks:
- `Modules/` - fake workload implementations such as `BootlogModule`
- `Orchestration/` - scheduler loop, context wiring, and delay controller
- `Helpers/` - utilities for names, timestamps, randomness, and progress bars
- `Data/` - abstractions plus JSON datasets compiled into the assembly

## Embedded Data
Domain-specific word lists live under `faketasks.Core/Data/Resources/*.json`. They are compiled as embedded resources and loaded through `EmbeddedDataProvider`, so no runtime downloads or configuration files are required.

## Development
```bash
dotnet restore
dotnet build
dotnet run --project faketasks.Cli -- bootlog --test
```

Tests have not been written yet; consider adding module-level unit tests before shipping new behaviour.

## Roadmap
- Cargo/Rust build simulation
- Docker image build simulation
- Terraform provisioning simulation

## License
MIT License (c) 2025 Damon Fedorick
