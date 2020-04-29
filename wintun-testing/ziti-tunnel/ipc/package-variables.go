package ipc

import (
	"sync"

	"github.com/michaelquigley/pfxlog"
	"golang.org/x/sys/windows/svc"
	"golang.org/x/sys/windows/svc/eventlog"

	"wintun-testing/ziti-tunnel/runtime"
)

var ipcPipeName = `\\.\pipe\NetFoundry\tunneler\ipc`
var logsPipeName = `\\.\pipe\NetFoundry\tunneler\logs`
var state = runtime.TunnelerState{}
var interrupt chan struct{}

var wg sync.WaitGroup
var connections int

const (
	SUCCESS              = 0
	COULD_NOT_WRITE_FILE = 1
	COULD_NOT_ENROLL     = 2

	UNKNOWN_ERROR          = 100
	ERROR_DISCONNECTING_ID = 50
	IDENTITY_NOT_FOUND     = 1000

	// see: https://docs.microsoft.com/en-us/windows/win32/secauthz/sid-strings
	// breaks down to
	//		"allow" 	  	 - A   (A;;
	// 	 	"full access" 	 - FA  (A;;FA
	//		"well-known sid" - IU  (A;;FA;;;IU)
	InteractivelyLoggedInUser = "(A;;GRGW;;;IU)" //generic read/write. We will want to tune this to a specific group but that is not working with Windows 10 home at the moment
	System                    = "(A;;FA;;;SY)"
	BuiltinAdmins             = "(A;;FA;;;BA)"
	LocalService              = "(A;;FA;;;LS)"

	cmdsAccepted = svc.AcceptStop | svc.AcceptShutdown | svc.AcceptPauseAndContinue
)

var Plog = pfxlog.Logger()
var log = Plog
var Elog *eventlog.Log

// This is the name you will use for the NET START command
const SvcName = "ziti"

// This is the name that will appear in the Services control panel
const SvcNameLong = SvcName + " long description here"