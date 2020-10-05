Namespace Enums.Map
    Public Enum TransferAbortReason As Short
        TRANSFER_ABORT_MAX_PLAYERS = &H1                ' Transfer Aborted: instance is full
        TRANSFER_ABORT_NOT_FOUND = &H2                  ' Transfer Aborted: instance not found
        TRANSFER_ABORT_TOO_MANY_INSTANCES = &H3         ' You have entered too many instances recently.
        TRANSFER_ABORT_ZONE_IN_COMBAT = &H5             ' Unable to zone in while an encounter is in progress.
        TRANSFER_ABORT_INSUF_EXPAN_LVL1 = &H106         ' You must have TBC expansion installed to access this area.
    End Enum
End NameSpace