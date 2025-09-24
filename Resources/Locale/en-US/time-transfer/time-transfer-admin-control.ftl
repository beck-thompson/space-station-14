time-transfer-admin-control-title = Time transfer control

# Server view
time-transfer-admin-control-cvar-enabled = Time transfers enabled:
time-transfer-admin-control-cvar-incoming = Allow new incoming time transfers:
time-transfer-admin-control-cvar-outgoing = Allow new outgoing time transfers:
time-transfer-admin-control-cvar-unverified = Allow unverified time transfers:
time-transfer-admin-control-cvar-private-key = This servers private key is (only updated on refresh):

# Server view
time-transfer-admin-control-view-name = Name:
time-transfer-admin-control-view-public-key = Public key:
time-transfer-admin-control-view-enabled = Enabled:

time-transfer-private-key-status = { $status ->
    [Valid] [color=green]VALID[/color]
    [Invalid] [color=red]INVALID[/color]
    [Empty] [color=yellow]EMPTY[/color]
    *[other] $status
}

time-transfer-true-false-color = { $bool ->
    [true] [color=green]TRUE[/color]
    [false] [color=red]FALSE[/color]
    *[other] $bool
}
