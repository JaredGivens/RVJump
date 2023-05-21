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
pub extern "C" fn emulator_cpu_execute(emu: *mut Emulator, executed_instruction: *mut u32) -> u32 {
    assert!(!emu.is_null());

    unsafe {
        match emu.as_mut().unwrap().cpu.execute() {
            Ok(v) => *executed_instruction = v as u32,
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

/* ASSEMBLER */
use deno_core::v8;
use deno_core::FastString;
use deno_core::JsRuntime;
use deno_core::RuntimeOptions;
use serde_json;
use serde_v8;
use std::ffi::c_char;
use std::ffi::CStr;
use std::ffi::CString;

#[no_mangle]
pub extern "C" fn free_riscv_assemble(bytes: *mut u8) {
    assert!(!bytes.is_null());

    unsafe {
        let _ = Box::from_raw(bytes);
    };
}

#[no_mangle]
pub extern "C" fn riscv_assemble(
    instruction: *const c_char,
    out: *mut *mut u8,
    error_line: *mut u64,
) -> u64 {
    unsafe { *error_line = 0 };

    let instructions = unsafe { CString::from(CStr::from_ptr(instruction)).into_string() };

    if instructions.is_err() {
        return 0;
    }

    let instructions = instructions.unwrap();

    let mut runtime = JsRuntime::new(RuntimeOptions::default());

    let instruction_setup = String::from(include_str!("../encoder/Instruction.js"));

    if let Err(_) = eval(&mut runtime, instruction_setup.into()) {
        return 0;
    }

    let mut instr_memory = Vec::new();

    for (i, instr) in instructions.split("\n").enumerate() {
        let wrapped_instr = format!(
            "\nnew Instruction('{}', {{ 'ISA': COPTS_ISA.RV32I }}).bin",
            instr
        );

        if let Ok(eval_result) = eval(&mut runtime, wrapped_instr.into()) {
            let instr_word = u32::from_str_radix(eval_result.as_str().unwrap(), 2).unwrap();
            instr_memory.extend(instr_word.to_le_bytes());
        } else {
            unsafe { *error_line = (i + 1) as u64 }
            return 0;
        }
    }

    let len = instr_memory.len();

    unsafe {
        let mut boxed = instr_memory.into_boxed_slice();
        *out = boxed.as_mut_ptr();
        std::mem::forget(boxed);
    }

    len as u64
}

fn eval(context: &mut JsRuntime, code: FastString) -> Result<serde_json::Value, String> {
    let res = context.execute_script("<anon>", code);
    match res {
        Ok(global) => {
            let scope = &mut context.handle_scope();
            let local = v8::Local::new(scope, global);
            // Deserialize a `v8` object into a Rust type using `serde_v8`,
            // in this case deserialize to a JSON `Value`.
            let deserialized_value = serde_v8::from_v8::<serde_json::Value>(scope, local);

            match deserialized_value {
                Ok(value) => Ok(value),
                Err(err) => Err(format!("Cannot deserialize value: {err:?}")),
            }
        }
        Err(err) => Err(format!("Evaling error: {err:?}")),
    }
}
/*

#[cfg(test)]
mod tests {
    use crate::riscv_assemble;
    use std::ffi::CString;

    #[test]
    fn test_add() {
        println!("{:?}", riscv_assemble(
            CString::new("add x1,x2,x3\nsub x1,x2,x3\naddi x1,x2,3")
                .unwrap()
                .as_ptr(),
        );
    });
}

*/
