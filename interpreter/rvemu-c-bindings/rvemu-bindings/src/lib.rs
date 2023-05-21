use rvemu::bus::DRAM_BASE;
use rvemu::emulator::Emulator;

pub fn new_emulator(program_bytes: Option<Vec<u8>>) -> Box<Emulator> {
    let mut emulator = Box::new(Emulator::new());

    if let Some(bytes) = program_bytes {
        emulator.as_mut().initialize_dram(bytes);
    }

    emulator.initialize_pc(DRAM_BASE);

    return emulator;
}

#[no_mangle]
pub extern "C" fn emulator_create() -> *mut Emulator {
    let emu = Box::new(Emulator::new());

    Box::into_raw(emu)
}

#[no_mangle]
pub extern "C" fn emulator_destroy(emu: *mut Emulator) {
    assert!(!emu.is_null());
    unsafe {
        let _ = Box::from_raw(emu);
    };
}

#[no_mangle]
pub extern "C" fn emulator_load_program(emu: *mut Emulator, program_bytes: *const u8, len: usize) {
    assert!(!emu.is_null());

    let mut program = vec![0; len];

    let slice = unsafe { std::slice::from_raw_parts(program_bytes, len) };

    program.clone_from_slice(slice);

    unsafe {
        emu.as_mut().unwrap().initialize_dram(program);
        emu.as_mut().unwrap().initialize_pc(DRAM_BASE);
    }
}

#[no_mangle]
pub extern "C" fn emulator_cpu_execute(emu: *mut Emulator, executed_instruction: *mut u64) -> u64 {
    assert!(!emu.is_null());

    unsafe {
        match emu.as_mut().unwrap().cpu.execute() {
            Ok(v) => *executed_instruction = v,
            Err(err) => {
                println!("{:?}", err);
                panic!()
            }
        };
    }

    0
}

#[no_mangle]
pub extern "C" fn emulator_get_register(emu: *mut Emulator, index: u64) -> u64 {
    unsafe { emu.as_mut().unwrap().cpu.xregs.read(index) }
}

#[no_mangle]
pub extern "C" fn emulator_set_register(emu: *mut Emulator, index: u64, value: u64) {
    unsafe {
        emu.as_mut().unwrap().cpu.xregs.write(index, value);
    }
}
